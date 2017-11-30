using Microsoft.AspNetCore.Builder;

namespace Lyfe
{
    public static class LyfeApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseLyfe(this IApplicationBuilder app)
            => app.UseMiddleware<LyfeMiddleware>();
    }
}
