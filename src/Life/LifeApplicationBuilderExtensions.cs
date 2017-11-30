using Microsoft.AspNetCore.Builder;

namespace Life
{
    public static class LifeApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseLife(this IApplicationBuilder app)
            => app.UseMiddleware<LifeMiddleware>();
    }
}
