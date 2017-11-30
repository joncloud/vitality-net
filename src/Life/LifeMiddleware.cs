using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Life
{
    class LifeMiddleware
    {
        readonly LifeOptions _options;
        readonly IStatusService _statusService;
        readonly RequestDelegate _next;
        public LifeMiddleware(IOptions<LifeOptions> options, IStatusService statusService, RequestDelegate next)
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
            if (context.Request.Path.StartsWithSegments(_options.Path))
            {
                // TODO add fork for /life/{component} to check for authorization and also report the details

                var statuses = await _statusService.EvaluateComponentsAsync();
                context.Response.StatusCode = GetStatusCodeFor(statuses.Values);

                string json = JsonConvert.SerializeObject(statuses);
                await context.Response.WriteAsync(json);
            }
            else
            {
                await _next(context);
            }
        }
    }
}
