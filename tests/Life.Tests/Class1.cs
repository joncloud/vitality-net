using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Life.Tests
{
    public class Class1
    {
        [Fact]
        public Task Test1() =>
            TestAsync(async http =>
            {
                var json = await http.GetStringAsync("/life");
                var statuses = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
                Assert.NotNull(statuses);
                Assert.NotEmpty(statuses);
                Assert.Contains("Sqlite", statuses.Keys);
                Assert.Equal("Up", statuses["Sqlite"]);
            });

        protected async Task TestAsync(Func<HttpClient, Task> fn)
        {
            var webHostBuilder = CreateWebHostBuilder();
            using (var server = new TestServer(webHostBuilder))
            using (var client = server.CreateClient())
            {
                await fn(client);
            }
        }
        

        IWebHostBuilder CreateWebHostBuilder()
            => new WebHostBuilder()
               .ConfigureServices(services =>
               {
                   services.AddLife(options => options.AddDbConnectionEvaluator("Sqlite", () => new SqliteConnection(), "Data Source=:memory:;"));
               })
               .Configure(app =>
               {
                   app.UseLife().Run(ctx => ctx.Response.WriteAsync("Hello World"));
               });
    }
}
