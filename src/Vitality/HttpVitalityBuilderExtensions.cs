using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Net.Http;
using System.Threading.Tasks;

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

        public static IVitalityBuilder AddHttpEvaluator(this IVitalityBuilder vitalityBuilder, string component, Func<HttpRequestMessage> requestFactory, Func<HttpResponseMessage, Task<bool>> fn)
        {
            vitalityBuilder.Services.TryAddSingleton<HttpEvaluator>();
            return vitalityBuilder.AddEvaluator<HttpEvaluator>(component, http => http.EvaluateAsync(component, requestFactory(), fn));
        }

        public static IVitalityBuilder AddHttpEvaluator(this IVitalityBuilder vitalityBuilder, string component, Func<HttpRequestMessage> requestFactory, Func<HttpResponseMessage, Task<bool>> fn, TimeSpan cacheAbsoluteExpiration)
        {
            vitalityBuilder.Services.TryAddSingleton<HttpEvaluator>();
            return vitalityBuilder.AddEvaluator<HttpEvaluator>(component, cacheAbsoluteExpiration, http => http.EvaluateAsync(component, requestFactory(), fn));
        }
    }
}
