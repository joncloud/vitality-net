using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vitality.AzureStorage
{
    class StorageQueueEvaluator
    {
        public async Task<ComponentStatus> EvaluateAsync(string component, CloudQueueClient client, Func<CloudQueueClient, Task<bool>> fn)
        {
            var details = new Dictionary<string, object>
            {
                ["Uri"] = client.BaseUri
            };

            return await fn(client)
                ? ComponentStatus.Up(component, details)
                : ComponentStatus.Down(component, details);
        }
    }
}