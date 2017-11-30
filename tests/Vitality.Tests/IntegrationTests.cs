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

namespace Vitality.Tests
{
    public class IntegrationTests
    {
        protected async Task TestAsync(Action<IVitalityBuilder> configure, Func<HttpClient, Task> fn)
        {
            var webHostBuilder = CreateWebHostBuilder(configure);
            using (var server = new TestServer(webHostBuilder))
            using (var client = server.CreateClient())
            {
                await fn(client);
            }
        }

        IWebHostBuilder CreateWebHostBuilder(Action<IVitalityBuilder> configure)
            => new WebHostBuilder()
               .ConfigureServices(services =>
               {
                   services.AddVitality(configure);
               })
               .Configure(app =>
               {
                   app.UseVitality().Run(ctx => ctx.Response.WriteAsync("Hello World"));
               });
    }
}
