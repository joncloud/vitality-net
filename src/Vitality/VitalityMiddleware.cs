using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace Vitality
{
    class VitalityMiddleware
    {
        readonly VitalityOptions _options;
        readonly IStatusService _statusService;
        readonly RequestDelegate _next;
        public VitalityMiddleware(IOptions<VitalityOptions> options, IStatusService statusService, RequestDelegate next)
        {
            _options = options.Value;
            _statusService = statusService;
            _next = next;
        }

        int GetStatusCodeFor(IEnumerable<string> statuses) =>
            statuses.Any(status => status != "Up")
                ? _options.NotUpStatusCode
                : _options.UpStatusCode;

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.StartsWithSegments(_options.Path, out var remaining))
            {
                Task task = remaining == PathString.Empty
                    ? WriteAllStatuses(context)
                    : WriteStatusFor(context, remaining.Value.Substring(1));
                
                await task;
            }
            else
            {
                await _next(context);
            }
        }

        async Task WriteStatusFor(HttpContext context, string component)
        {
            bool authorized = await _options.AuthorizeDetails(context);
            if (authorized)
            {
                var status = await _statusService.EvaluateComponentAsync(component);
                // TODO remove array
                context.Response.StatusCode = GetStatusCodeFor(new[] { status.Status });
                await WriteJsonAsync(context, status);
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            }
        }

        async Task WriteAllStatuses(HttpContext context)
        {
            var statuses = await _statusService.EvaluateComponentsAsync();
            context.Response.StatusCode = GetStatusCodeFor(statuses.Values);

            await WriteJsonAsync(context, statuses);
        }

        async Task WriteJsonAsync<T>(HttpContext context, T value)
        {
            context.Response.ContentType = "application/json";
            string json = JsonConvert.SerializeObject(value, _options.JsonSettings);
            await context.Response.WriteAsync(json);
        }
    }
}
