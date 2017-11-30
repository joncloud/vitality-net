using Microsoft.AspNetCore.Http;

namespace Life
{
    public class LifeOptions
    {
        public PathString Path { get; set; }
        public int UpStatusCode { get; set; } = StatusCodes.Status200OK;
        public int NotUpStatusCode { get; set; } = StatusCodes.Status200OK;
    }
}
