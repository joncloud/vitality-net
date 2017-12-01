using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Vitality
{
    public static class HttpVitalityBuilderExtensions
    {
        public static IVitalityBuilder AddHttpEvaluator(this IVitalityBuilder vitalityBuilder, string component, string uri)
        {
            vitalityBuilder.Services.TryAddSingleton<HttpEvaluator>();
            return vitalityBuilder.AddEvaluator<HttpEvaluator>(component, http => http.EvaluateAsync(component, uri));
        }

        public static IVitalityBuilder AddHttpEvaluator(this IVitalityBuilder vitalityBuilder, string component, string uri, TimeSpan cacheAbsoluteExpiration)
        {
            vitalityBuilder.Services.TryAddSingleton<HttpEvaluator>();
            return vitalityBuilder.AddEvaluator<HttpEvaluator>(component, cacheAbsoluteExpiration, http => http.EvaluateAsync(component, uri));
        }
    }
}
