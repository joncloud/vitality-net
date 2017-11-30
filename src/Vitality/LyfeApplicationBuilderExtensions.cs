using Microsoft.AspNetCore.Builder;

namespace Vitality
{
    public static class VitalityApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseVitality(this IApplicationBuilder app)
            => app.UseMiddleware<VitalityMiddleware>();
    }
}
