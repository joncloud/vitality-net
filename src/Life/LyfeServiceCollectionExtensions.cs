using Microsoft.Extensions.DependencyInjection;
using System;

namespace Lyfe
{
    public static class LyfeServiceCollectionExtensions
    {
        public static IServiceCollection AddLyfe(this IServiceCollection services, Action<ILyfeBuilder> configure)
        {
            var builder = new LyfeBuilder(services);
            configure(builder);
            return services.Configure<LyfeOptions>(builder.Configure)
                .AddMemoryCache()
                .AddSingleton<IStatusService, StatusService>();
        }
    }
}
