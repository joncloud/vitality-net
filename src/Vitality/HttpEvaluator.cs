using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Vitality
{
    class HttpEvaluator
    {
        readonly HttpClient _http;
        public HttpEvaluator()
        {
            _http = new HttpClient();
            _http.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };
        }

        public Task<ComponentStatus> EvaluateAsync(string component, string uri) =>
            EvaluateAsync(component, new HttpRequestMessage(HttpMethod.Get, uri), resp => Task.FromResult(resp.IsSuccessStatusCode));

        public async Task<ComponentStatus> EvaluateAsync(string component, HttpRequestMessage request, Func<HttpResponseMessage, Task<bool>> fn)
        {
            var response = await _http.SendAsync(request).ConfigureAwait(false);

            var details = new Dictionary<string, object>
            {
                ["Uri"] = request.RequestUri,
                ["Method"] = request.Method,
                ["StatusCode"] = response.StatusCode
            };

            var up = await fn(response);
            return up
                ? ComponentStatus.Up(component, details)
                : ComponentStatus.Down(component, details);
        }
    }
}
