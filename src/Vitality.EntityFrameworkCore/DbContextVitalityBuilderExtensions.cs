using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Threading.Tasks;

namespace Vitality.EntityFrameworkCore
{
    public static class DbContextVitalityBuilderExtensions
    {
        public static IVitalityBuilder AddDbContextEvaluator<T>(this IVitalityBuilder vitalityBuilder, string component, Func<T, Task> fn)
            where T : DbContext =>
            vitalityBuilder.AddDbContextEvaluator<T>(component, async ctx => { await fn(ctx); return true; });

        public static IVitalityBuilder AddDbContextEvaluator<T>(this IVitalityBuilder vitalityBuilder, string component, Func<T, Task<bool>> fn)
            where T : DbContext
        {
            vitalityBuilder.Services.TryAddSingleton<DbContextEvaluator<T>>();
            return vitalityBuilder.AddEvaluator<DbContextEvaluator<T>>(component, eval => eval.EvaluateAsync(component, fn));
        }

        public static IVitalityBuilder AddDbContextEvaluator<T>(this IVitalityBuilder vitalityBuilder, string component, Func<T, Task> fn, TimeSpan cacheAbsoluteExpiration)
            where T : DbContext =>
            vitalityBuilder.AddDbContextEvaluator<T>(component, async ctx => { await fn(ctx); return true; }, cacheAbsoluteExpiration);

        public static IVitalityBuilder AddDbContextEvaluator<T>(this IVitalityBuilder vitalityBuilder, string component, Func<T, Task<bool>> fn, TimeSpan cacheAbsoluteExpiration)
            where T : DbContext
        {
            vitalityBuilder.Services.TryAddSingleton<DbContextEvaluator<T>>();
            return vitalityBuilder.AddEvaluator<DbContextEvaluator<T>>(component, cacheAbsoluteExpiration, eval => eval.EvaluateAsync(component, fn));
        }

        public static IVitalityBuilder AddDbContextEvaluator<T>(this IVitalityBuilder vitalityBuilder, string component)
            where T : DbContext =>
            vitalityBuilder.AddDbContextEvaluator<T>(component, "SELECT 1");

        public static IVitalityBuilder AddDbContextEvaluator<T>(this IVitalityBuilder vitalityBuilder, string component, TimeSpan cacheAbsoluteExpiration)
            where T : DbContext =>
            vitalityBuilder.AddDbContextEvaluator<T>(component, "SELECT 1", cacheAbsoluteExpiration);

        public static IVitalityBuilder AddDbContextEvaluator<T>(this IVitalityBuilder vitalityBuilder, string component, string commandText)
            where T : DbContext
        {
            vitalityBuilder.Services.TryAddSingleton<DbContextEvaluator<T>>();
            return vitalityBuilder.AddEvaluator<DbContextEvaluator<T>>(component, eval => eval.EvaluateAsync(component, commandText));
        }

        public static IVitalityBuilder AddDbContextEvaluator<T>(this IVitalityBuilder vitalityBuilder, string component, string commandText, TimeSpan cacheAbsoluteExpiration)
            where T : DbContext
        {
            vitalityBuilder.Services.TryAddSingleton<DbContextEvaluator<T>>();
            return vitalityBuilder.AddEvaluator<DbContextEvaluator<T>>(component, cacheAbsoluteExpiration, eval => eval.EvaluateAsync(component, commandText));
        }
    }
}
