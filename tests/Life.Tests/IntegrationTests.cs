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
using System.Threading.Tasks;
using Xunit;

namespace Life.Tests
{
    public class IntegrationTests
    {
        protected async Task TestAsync(Action<ILifeBuilder> configure, Func<HttpClient, Task> fn)
        {
            var webHostBuilder = CreateWebHostBuilder(configure);
            using (var server = new TestServer(webHostBuilder))
            using (var client = server.CreateClient())
            {
                await fn(client);
            }
        }

        IWebHostBuilder CreateWebHostBuilder(Action<ILifeBuilder> configure)
            => new WebHostBuilder()
               .ConfigureServices(services =>
               {
                   services.AddLife(configure);
               })
               .Configure(app =>
               {
                   app.UseLife().Run(ctx => ctx.Response.WriteAsync("Hello World"));
               });
    }
}
