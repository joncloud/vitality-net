using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Data.Common;

namespace Life
{
    public static class DbConnectionLifeBuilderExtensions
    {
        public static ILifeBuilder AddDbConnectionEvaluator<T>(this ILifeBuilder lifeBuilder, string component, Func<T> connectionFactory, string connectionString)
            where T : DbConnection =>
            lifeBuilder.AddDbConnectionEvaluator(component, connectionFactory, connectionString, "SELECT 1");

        public static ILifeBuilder AddDbConnectionEvaluator<T>(this ILifeBuilder lifeBuilder, string component, Func<T> connectionFactory, string connectionString, TimeSpan cacheAbsoluteExpiration)
            where T : DbConnection =>
            lifeBuilder.AddDbConnectionEvaluator(component, connectionFactory, connectionString, "SELECT 1", cacheAbsoluteExpiration);

        public static ILifeBuilder AddDbConnectionEvaluator<T>(this ILifeBuilder lifeBuilder, string component, Func<T> connectionFactory, string connectionString, string commandText)
            where T : DbConnection
        {
            lifeBuilder.Services.TryAddSingleton(svc => new DbConnectionEvaluator<T>(connectionFactory));
            var options = new DbConnectionOptions(component, connectionString, commandText);
            return lifeBuilder.AddEvaluator<DbConnectionEvaluator<T>>(component, db => db.EvaluateAsync(options));
        }

        public static ILifeBuilder AddDbConnectionEvaluator<T>(this ILifeBuilder lifeBuilder, string component, Func<T> connectionFactory, string connectionString, string commandText, TimeSpan cacheAbsoluteExpiration)
            where T : DbConnection
        {
            lifeBuilder.Services.TryAddSingleton(svc => new DbConnectionEvaluator<T>(connectionFactory));
            var options = new DbConnectionOptions(component, connectionString, commandText);
            return lifeBuilder.AddEvaluator<DbConnectionEvaluator<T>>(component, cacheAbsoluteExpiration, db => db.EvaluateAsync(options));
        }
    }
}
