using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Vitality.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Vitality.Tests
{
    class EFCoreTestDbContext : DbContext
    {
        public EFCoreTestDbContext(DbContextOptions<EFCoreTestDbContext> options)
            : base(options) { }

        public DbSet<EFCoreModel> EFCoreModels { get; set; }
    }

    public class EFCoreModel
    {
        public int Id { get; set; }
        public string Data { get; set; }
    }

    public class EFCoreTests : IntegrationTests
    {
        [Fact]
        public Task ShouldReportEFCoreCommandIsUp() =>
            TestAsync(UseEFCoreCommandTests, http => AssertStatus(http, "efCore", "Up"));

        static void UseEFCoreCommandTests(IVitalityBuilder options) =>
            options.AddDbContextEvaluator<EFCoreTestDbContext>("EFCore").Services.AddDbContext<EFCoreTestDbContext>(o => o.UseSqlite("Data Source=:memory:;"));

        [Fact]
        public Task ShouldReportEFCoreFuncIsUp() =>
            TestAsync(UseEFCoreCommandTests, http => AssertStatus(http, "efCore", "Up"));

        static void UseEFCoreFuncTests(IVitalityBuilder options) =>
            options.AddDbContextEvaluator<EFCoreTestDbContext>("EFCore", new Func<EFCoreTestDbContext, Task>(TestEFCore)).Services.AddDbContext<EFCoreTestDbContext>(o => o.UseSqlite("Data Source=:memory:;"));

        static Task TestEFCore(EFCoreTestDbContext context) =>
            context.EFCoreModels.AnyAsync();

        static async Task AssertStatus(HttpClient http, string component, string status)
        {
            var json = await http.GetStringAsync("/vitality");
            var statuses = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            Assert.NotNull(statuses);
            Assert.NotEmpty(statuses);
            Assert.Contains(component, statuses.Keys);
            Assert.Equal(status, statuses[component]);
        }
    }
}
