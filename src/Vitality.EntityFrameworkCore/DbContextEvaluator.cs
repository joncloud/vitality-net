using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Vitality.EntityFrameworkCore
{
    class DbContextEvaluator<T> where T : DbContext
    {
        readonly T _context;
        public DbContextEvaluator(T context) =>
            _context = context;

        public async Task<ComponentStatus> EvaluateAsync(string component, Func<T, Task<bool>> fn)
        {
            var details = new Dictionary<string, object>
            {
                ["ConnectionString"] = _context.Database.GetDbConnection().ConnectionString,
                ["ContextType"] = typeof(T).GetType()
            };

            return await fn(_context)
                ? ComponentStatus.Up(component, details)
                : ComponentStatus.Down(component, details);
        }

        public async Task<ComponentStatus> EvaluateAsync(string component, string commandText)
        {
            var connection = _context.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open) await connection.OpenAsync();
            var command = connection.CreateCommand();
            command.CommandText = commandText;
            using (var reader = await command.ExecuteReaderAsync())
            {
                var details = new Dictionary<string, object>
                {
                    ["ConnectionString"] = connection.ConnectionString,
                    ["CommandText"] = commandText,
                    ["ContextType"] = typeof(T).GetType()
                };

                return reader.HasRows
                    ? ComponentStatus.Up(component, details)
                    : ComponentStatus.Down(component, details);
            }
        }
    }
}
