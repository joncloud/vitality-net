using Microsoft.WindowsAzure.Storage.File;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vitality.AzureStorage
{
    class StorageFileEvaluator
    {
        public async Task<ComponentStatus> EvaluateAsync(string component, CloudFileClient client, Func<CloudFileClient, Task<bool>> fn)
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
