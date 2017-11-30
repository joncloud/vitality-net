using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Life
{
    public static class HttpLifeBuilderExtensions
    {
        public static ILifeBuilder AddHttpEvaluator(this ILifeBuilder lifeBuilder, string component, string uri)
        {
            lifeBuilder.Services.TryAddSingleton<HttpEvaluator>();
            return lifeBuilder.AddEvaluator<HttpEvaluator>(component, http => http.EvaluateAsync(component, uri));
        }

        public static ILifeBuilder AddHttpEvaluator(this ILifeBuilder lifeBuilder, string component, string uri, TimeSpan cacheAbsoluteExpiration)
        {
            lifeBuilder.Services.TryAddSingleton<HttpEvaluator>();
            return lifeBuilder.AddEvaluator<HttpEvaluator>(component, cacheAbsoluteExpiration, http => http.EvaluateAsync(component, uri));
        }
    }
}
