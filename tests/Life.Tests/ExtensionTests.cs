using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Lyfe.Tests
{
    public class ExtensionTests : IntegrationTests
    {
        [Fact]
        public Task ShouldReportGoogleIsUp() =>
            TestAsync(UseGoogle, http => AssertStatus(http, "google", "Up"));

        [Fact]
        public Task ShouldReportSqliteIsUp() =>
            TestAsync(UseSqlite, http => AssertStatus(http, "sqlite", "Up"));

        static void UseSqlite(ILyfeBuilder options) =>
            options.AddDbConnectionEvaluator("Sqlite", () => new SqliteConnection(), "Data Source=:memory:;");

        static void UseGoogle(ILyfeBuilder options) =>
            options.AddHttpEvaluator("Google", "https://www.google.com");

        static async Task AssertStatus(HttpClient http, string component, string status)
        {
            var json = await http.GetStringAsync("/lyfe");
            var statuses = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            Assert.NotNull(statuses);
            Assert.NotEmpty(statuses);
            Assert.Contains(component, statuses.Keys);
            Assert.Equal(status, statuses[component]);
        }
    }
}
