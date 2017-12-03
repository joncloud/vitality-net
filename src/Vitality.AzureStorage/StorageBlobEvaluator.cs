using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vitality.AzureStorage
{
    class StorageBlobEvaluator
    {
        public async Task<ComponentStatus> EvaluateAsync(string component, CloudBlobClient client, Func<CloudBlobClient, Task<bool>> fn)
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