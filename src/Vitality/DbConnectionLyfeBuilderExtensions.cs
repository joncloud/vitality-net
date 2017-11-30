using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Data.Common;

namespace Vitality
{
    public static class DbConnectionVitalityBuilderExtensions
    {
        public static IVitalityBuilder AddDbConnectionEvaluator<T>(this IVitalityBuilder vitalityBuilder, string component, Func<T> connectionFactory, string connectionString)
            where T : DbConnection =>
            vitalityBuilder.AddDbConnectionEvaluator(component, connectionFactory, connectionString, "SELECT 1");

        public static IVitalityBuilder AddDbConnectionEvaluator<T>(this IVitalityBuilder vitalityBuilder, string component, Func<T> connectionFactory, string connectionString, TimeSpan cacheAbsoluteExpiration)
            where T : DbConnection =>
            vitalityBuilder.AddDbConnectionEvaluator(component, connectionFactory, connectionString, "SELECT 1", cacheAbsoluteExpiration);

        public static IVitalityBuilder AddDbConnectionEvaluator<T>(this IVitalityBuilder vitalityBuilder, string component, Func<T> connectionFactory, string connectionString, string commandText)
            where T : DbConnection
        {
            vitalityBuilder.Services.TryAddSingleton(svc => new DbConnectionEvaluator<T>(connectionFactory));
            var options = new DbConnectionOptions(component, connectionString, commandText);
            return vitalityBuilder.AddEvaluator<DbConnectionEvaluator<T>>(component, db => db.EvaluateAsync(options));
        }

        public static IVitalityBuilder AddDbConnectionEvaluator<T>(this IVitalityBuilder vitalityBuilder, string component, Func<T> connectionFactory, string connectionString, string commandText, TimeSpan cacheAbsoluteExpiration)
            where T : DbConnection
        {
            vitalityBuilder.Services.TryAddSingleton(svc => new DbConnectionEvaluator<T>(connectionFactory));
            var options = new DbConnectionOptions(component, connectionString, commandText);
            return vitalityBuilder.AddEvaluator<DbConnectionEvaluator<T>>(component, cacheAbsoluteExpiration, db => db.EvaluateAsync(options));
        }
    }
}
