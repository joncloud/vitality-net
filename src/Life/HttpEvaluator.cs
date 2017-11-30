using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace Lyfe
{
    class HttpEvaluator
    {
        readonly HttpClient _http;
        public HttpEvaluator()
        {
            _http = new HttpClient();
            _http.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };
        }

        public async Task<ComponentStatus> EvaluateAsync(string component, string uri)
        {
            var response = await _http.GetAsync(uri).ConfigureAwait(false);

            var details = new Dictionary<string, object>
            {
                ["Uri"] = uri,
                ["StatusCode"] = response.StatusCode
            };

            return response.IsSuccessStatusCode
                ? ComponentStatus.Up(component, details)
                : ComponentStatus.Down(component, details);
        }
    }
}
