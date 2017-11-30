using Microsoft.Extensions.DependencyInjection;
using System;

namespace Vitality
{
    public static class VitalityServiceCollectionExtensions
    {
        public static IServiceCollection AddVitality(this IServiceCollection services, Action<IVitalityBuilder> configure)
        {
            var builder = new VitalityBuilder(services);
            configure(builder);
            return services.Configure<VitalityOptions>(builder.Configure)
                .AddMemoryCache()
                .AddSingleton<IStatusService, StatusService>();
        }
    }
}
