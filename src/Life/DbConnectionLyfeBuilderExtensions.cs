using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Data.Common;

namespace Lyfe
{
    public static class DbConnectionLyfeBuilderExtensions
    {
        public static ILyfeBuilder AddDbConnectionEvaluator<T>(this ILyfeBuilder lyfeBuilder, string component, Func<T> connectionFactory, string connectionString)
            where T : DbConnection =>
            lyfeBuilder.AddDbConnectionEvaluator(component, connectionFactory, connectionString, "SELECT 1");

        public static ILyfeBuilder AddDbConnectionEvaluator<T>(this ILyfeBuilder lyfeBuilder, string component, Func<T> connectionFactory, string connectionString, TimeSpan cacheAbsoluteExpiration)
            where T : DbConnection =>
            lyfeBuilder.AddDbConnectionEvaluator(component, connectionFactory, connectionString, "SELECT 1", cacheAbsoluteExpiration);

        public static ILyfeBuilder AddDbConnectionEvaluator<T>(this ILyfeBuilder lyfeBuilder, string component, Func<T> connectionFactory, string connectionString, string commandText)
            where T : DbConnection
        {
            lyfeBuilder.Services.TryAddSingleton(svc => new DbConnectionEvaluator<T>(connectionFactory));
            var options = new DbConnectionOptions(component, connectionString, commandText);
            return lyfeBuilder.AddEvaluator<DbConnectionEvaluator<T>>(component, db => db.EvaluateAsync(options));
        }

        public static ILyfeBuilder AddDbConnectionEvaluator<T>(this ILyfeBuilder lyfeBuilder, string component, Func<T> connectionFactory, string connectionString, string commandText, TimeSpan cacheAbsoluteExpiration)
            where T : DbConnection
        {
            lyfeBuilder.Services.TryAddSingleton(svc => new DbConnectionEvaluator<T>(connectionFactory));
            var options = new DbConnectionOptions(component, connectionString, commandText);
            return lyfeBuilder.AddEvaluator<DbConnectionEvaluator<T>>(component, cacheAbsoluteExpiration, db => db.EvaluateAsync(options));
        }
    }
}
