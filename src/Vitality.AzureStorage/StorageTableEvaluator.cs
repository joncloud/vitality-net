using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vitality.AzureStorage
{
    class StorageTableEvaluator
    {
        public async Task<ComponentStatus> EvaluateAsync(string component, CloudTableClient client, Func<CloudTableClient, Task<bool>> fn)
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