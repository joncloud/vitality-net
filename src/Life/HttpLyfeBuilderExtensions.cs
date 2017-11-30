using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Lyfe
{
    public static class HttpLyfeBuilderExtensions
    {
        public static ILyfeBuilder AddHttpEvaluator(this ILyfeBuilder lyfeBuilder, string component, string uri)
        {
            lyfeBuilder.Services.TryAddSingleton<HttpEvaluator>();
            return lyfeBuilder.AddEvaluator<HttpEvaluator>(component, http => http.EvaluateAsync(component, uri));
        }

        public static ILyfeBuilder AddHttpEvaluator(this ILyfeBuilder lyfeBuilder, string component, string uri, TimeSpan cacheAbsoluteExpiration)
        {
            lyfeBuilder.Services.TryAddSingleton<HttpEvaluator>();
            return lyfeBuilder.AddEvaluator<HttpEvaluator>(component, cacheAbsoluteExpiration, http => http.EvaluateAsync(component, uri));
        }
    }
}
