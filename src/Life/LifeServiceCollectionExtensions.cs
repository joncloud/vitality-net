using Microsoft.Extensions.DependencyInjection;
using System;

namespace Life
{
    public static class LifeServiceCollectionExtensions
    {
        public static IServiceCollection AddLife(this IServiceCollection services, Action<ILifeBuilder> configure)
        {
            var builder = new LifeBuilder(services);
            configure(builder);
            return services.Configure<LifeOptions>(builder.Configure)
                .AddMemoryCache()
                .AddSingleton<IStatusService, StatusService>();
        }
    }
}
