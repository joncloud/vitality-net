using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Lyfe
{
    public class LyfeOptions
    {
        public PathString Path { get; set; }
        public int UpStatusCode { get; set; } = StatusCodes.Status200OK;
        public int NotUpStatusCode { get; set; } = StatusCodes.Status200OK;
        public Func<HttpContext, Task<bool>> AuthorizeDetails { get; set; }
        public JsonSerializerSettings JsonSettings { get; set; }
    }
}
