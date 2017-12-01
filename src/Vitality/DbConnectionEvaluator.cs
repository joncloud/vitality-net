using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace Vitality
{
    class DbConnectionEvaluatorOptions<T> where T : DbConnection
    {
        public Func<T> ConnectionFactory { get; set; }
        public string CommandText { get; set; }
    }

    class DbConnectionEvaluator<T> : DbConnectionEvaluator where T : DbConnection
    {
        public DbConnectionEvaluator(Func<T> factory)
            : base(factory) { }
    }

    struct DbConnectionOptions
    {
        public string Component { get; }
        public string ConnectionString { get; }
        public string CommandText { get; }

        public DbConnectionOptions(string component, string connectionString, string commandText) : this()
        {
            Component = component;
            ConnectionString = connectionString;
            CommandText = commandText;
        }
    }

    class DbConnectionEvaluator
    {
        readonly Func<DbConnection> _factory;
        public DbConnectionEvaluator(Func<DbConnection> factory) =>
            _factory = factory;

        public async Task<ComponentStatus> EvaluateAsync(DbConnectionOptions options)
        {
            using (var connection = _factory())
            {
                connection.ConnectionString = options.ConnectionString;
                await connection.OpenAsync();
                var command = connection.CreateCommand();
                command.CommandText = options.CommandText;
                using (var reader = await command.ExecuteReaderAsync())
                {
                    var details = new Dictionary<string, object>
                    {
                        ["ConnectionString"] = connection.ConnectionString,
                        ["CommandText"] = options.CommandText,
                        ["ConnectionType"] = connection.GetType()
                    };

                    return reader.HasRows
                        ? ComponentStatus.Up(options.Component, details)
                        : ComponentStatus.Down(options.Component, details);
                }
            }
        }
    }
}
