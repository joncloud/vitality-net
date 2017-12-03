using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Vitality.AzureStorage;
using Xunit;

namespace Vitality.Tests
{
    public class AzureStorageTests : IntegrationTests
    {
        [AzureStorageFact]
        public Task ShouldReportAzureStorageBlobIsUp() =>
            TestAsync(UseAzureStorageBlob, http => AssertStatus(http, "azureStorageBlob", "Up"));

        static void UseAzureStorageBlob(IVitalityBuilder options) =>
            options.AddStorageBlobEvaluator("AzureStorageBlob", AzureStorageFactAttribute.CloudStorageAccount);

        [AzureStorageFact]
        public Task ShouldReportAzureStorageFileIsUp() =>
            TestAsync(UseAzureStorageFile, http => AssertStatus(http, "azureStorageFile", "Up"));

        static void UseAzureStorageFile(IVitalityBuilder options) =>
            options.AddStorageFileEvaluator("AzureStorageFile", AzureStorageFactAttribute.CloudStorageAccount);

        [AzureStorageFact]
        public Task ShouldReportAzureStorageQueueIsUp() =>
            TestAsync(UseAzureStorageQueueEmpty, http => AssertStatus(http, "azureStorageQueue", "Up"));

        static void UseAzureStorageQueueEmpty(IVitalityBuilder options) =>
            UseAzureStorageQueue("empty", options);

        [AzureStorageFact]
        public Task ShouldReportAzureStorageQueueIsDown() =>
            TestAsync(UseAzureStorageQueueOverflowing, async http =>
            {
                var account = AzureStorageFactAttribute.CloudStorageAccount;
                var queue = account.CreateCloudQueueClient().GetQueueReference("overflowing");
                await queue.FetchAttributesAsync();

                var tasks = Enumerable.Range(0, 15 - (queue.ApproximateMessageCount ?? 0))
                    .Select(_ => new CloudQueueMessage("a"))
                    .Select(msg => queue.AddMessageAsync(msg));
                await Task.WhenAll(tasks);

                await AssertStatus(http, "azureStorageQueue", "Down");
            });

        static void UseAzureStorageQueueOverflowing(IVitalityBuilder options) =>
            UseAzureStorageQueue("overflowing", options);

        static void UseAzureStorageQueue(string queueName, IVitalityBuilder options) =>
            options.AddStorageQueueEvaluator("AzureStorageQueue", AzureStorageFactAttribute.CloudStorageAccount, queueName, count => (count ?? 0) < 10);

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

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class AzureStorageFactAttribute : FactAttribute
    {
        public override string Skip
        {
            get { return _unavailable ?? base.Skip; }
            set { base.Skip = value; }
        }

        public static CloudStorageAccount CloudStorageAccount { get; }

        private static readonly string _unavailable;

        static AzureStorageFactAttribute()
        {
            try
            {
                var env = Environment.GetEnvironmentVariables();
                const string key = "Vitality__ConnectionStrings__AzureStorageAccount";
                if (!env.Contains(key))
                    _unavailable = $"Missing Environment Variable {key}";
                else if (CloudStorageAccount.TryParse(env[key].ToString(), out var account))
                    CloudStorageAccount = account;
                else
                    _unavailable = $"Invalid Connection String in Environment Variable {key}";
            }
            catch (Exception ex)
            {
                _unavailable = $"Failed to parse Storage Account: {ex.Message}";
            }
        }
    }
}
