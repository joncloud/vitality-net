using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Vitality.Tests
{
    public class ExtensionTests : IntegrationTests
    {
        [Fact]
        public Task ShouldReportGetGoogleIsUp() =>
            TestAsync(UseGetGoogle, http => AssertStatus(http, "google", "Up"));

        static void UseGetGoogle(IVitalityBuilder options) =>
            options.AddHttpEvaluator("Google", "https://www.google.com");

        [Fact]
        public Task ShouldReportHeadGoogleIsUp() =>
            TestAsync(UseHeadGoogle, http => AssertStatus(http, "google", "Up"));

        static void UseHeadGoogle(IVitalityBuilder options) =>
            options.AddHttpEvaluator("Google", () => new HttpRequestMessage(HttpMethod.Head, "https://www.google.com"), resp => Task.FromResult(resp.IsSuccessStatusCode));

        [Fact]
        public Task ShouldReportSqliteIsUp() =>
            TestAsync(UseSqlite, http => AssertStatus(http, "sqlite", "Up"));

        static void UseSqlite(IVitalityBuilder options) =>
            options.AddDbConnectionEvaluator("Sqlite", () => new SqliteConnection(), "Data Source=:memory:;");

        static async Task AssertStatus(HttpClient http, string component, string status)
        {
            var response = await http.GetAsync("/vitality");
            Assert.Equal("application/json", response.Content.Headers.ContentType.MediaType);
            var json = await response.Content.ReadAsStringAsync();
            var statuses = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            Assert.NotNull(statuses);
            Assert.NotEmpty(statuses);
            Assert.Contains(component, statuses.Keys);
            Assert.Equal(status, statuses[component]);
        }
    }
}
