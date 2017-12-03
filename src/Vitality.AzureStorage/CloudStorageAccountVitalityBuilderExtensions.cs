using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.File;
using Microsoft.WindowsAzure.Storage.Queue;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Threading.Tasks;

namespace Vitality.AzureStorage
{
    public static class CloudStorageAccountVitalityBuilderExtensions
    {
        public static IVitalityBuilder AddStorageBlobEvaluator(this IVitalityBuilder vitalityBuilder, string component, CloudStorageAccount account) =>
            vitalityBuilder.AddStorageBlobEvaluator(component, account, async client =>
            {
                await client.ListContainersSegmentedAsync(new BlobContinuationToken());
                return true;
            });

        public static IVitalityBuilder AddStorageBlobEvaluator(this IVitalityBuilder vitalityBuilder, string component, CloudStorageAccount account, TimeSpan cacheAbsoluteExpiration) =>
            vitalityBuilder.AddStorageBlobEvaluator(component, account, async client =>
            {
                await client.ListContainersSegmentedAsync(new BlobContinuationToken());
                return true;
            }, cacheAbsoluteExpiration);

        public static IVitalityBuilder AddStorageBlobEvaluator(this IVitalityBuilder vitalityBuilder, string component, CloudStorageAccount account, string containerName) =>
            vitalityBuilder.AddStorageBlobEvaluator(component, account, client => client.GetContainerReference(containerName).ExistsAsync());

        public static IVitalityBuilder AddStorageBlobEvaluator(this IVitalityBuilder vitalityBuilder, string component, CloudStorageAccount account, string containerName, TimeSpan cacheAbsoluteExpiration) =>
            vitalityBuilder.AddStorageBlobEvaluator(component, account, client => client.GetContainerReference(containerName).ExistsAsync(), cacheAbsoluteExpiration);

        public static IVitalityBuilder AddStorageBlobEvaluator(this IVitalityBuilder vitalityBuilder, string component, CloudStorageAccount account, Func<CloudBlobClient, Task<bool>> fn)
        {
            vitalityBuilder.Services.TryAddSingleton<StorageBlobEvaluator>();
            var client = account.CreateCloudBlobClient();
            return vitalityBuilder.AddEvaluator<StorageBlobEvaluator>(component, eval => eval.EvaluateAsync(component, client, fn));
        }

        public static IVitalityBuilder AddStorageBlobEvaluator(this IVitalityBuilder vitalityBuilder, string component, CloudStorageAccount account, Func<CloudBlobClient, Task<bool>> fn, TimeSpan cacheAbsoluteExpiration)
        {
            vitalityBuilder.Services.TryAddSingleton<StorageBlobEvaluator>();
            var client = account.CreateCloudBlobClient();
            return vitalityBuilder.AddEvaluator<StorageBlobEvaluator>(component, cacheAbsoluteExpiration, eval => eval.EvaluateAsync(component, client, fn));
        }

        public static IVitalityBuilder AddStorageFileEvaluator(this IVitalityBuilder vitalityBuilder, string component, CloudStorageAccount account) =>
            vitalityBuilder.AddStorageFileEvaluator(component, account, async client =>
            {
                await client.GetServicePropertiesAsync();
                return true;
            });

        public static IVitalityBuilder AddStorageFileEvaluator(this IVitalityBuilder vitalityBuilder, string component, CloudStorageAccount account, TimeSpan cacheAbsoluteExpiration) =>
            vitalityBuilder.AddStorageFileEvaluator(component, account, async client =>
            {
                await client.GetServicePropertiesAsync();
                return true;
            }, cacheAbsoluteExpiration);

        public static IVitalityBuilder AddStorageFileEvaluator(this IVitalityBuilder vitalityBuilder, string component, CloudStorageAccount account, Func<CloudFileClient, Task<bool>> fn)
        {
            vitalityBuilder.Services.TryAddSingleton<StorageFileEvaluator>();
            var client = account.CreateCloudFileClient();
            return vitalityBuilder.AddEvaluator<StorageFileEvaluator>(component, eval => eval.EvaluateAsync(component, client, fn));
        }

        public static IVitalityBuilder AddStorageFileEvaluator(this IVitalityBuilder vitalityBuilder, string component, CloudStorageAccount account, Func<CloudFileClient, Task<bool>> fn, TimeSpan cacheAbsoluteExpiration)
        {
            vitalityBuilder.Services.TryAddSingleton<StorageFileEvaluator>();
            var client = account.CreateCloudFileClient();
            return vitalityBuilder.AddEvaluator<StorageFileEvaluator>(component, cacheAbsoluteExpiration, eval => eval.EvaluateAsync(component, client, fn));
        }

        public static IVitalityBuilder AddStorageQueueEvaluator(this IVitalityBuilder vitalityBuilder, string component, CloudStorageAccount account, string queueName) =>
            vitalityBuilder.AddStorageQueueEvaluator(component, account, client => client.GetQueueReference(queueName).ExistsAsync());

        public static IVitalityBuilder AddStorageQueueEvaluator(this IVitalityBuilder vitalityBuilder, string component, CloudStorageAccount account, string queueName, TimeSpan cacheAbsoluteExpiration) =>
            vitalityBuilder.AddStorageQueueEvaluator(component, account, client => client.GetQueueReference(queueName).ExistsAsync(), cacheAbsoluteExpiration);

        public static IVitalityBuilder AddStorageQueueEvaluator(this IVitalityBuilder vitalityBuilder, string component, CloudStorageAccount account, string queueName, Func<int?, bool> fn) =>
            vitalityBuilder.AddStorageQueueEvaluator(component, account, async client =>
            {
                var queue = client.GetQueueReference(queueName);
                await queue.FetchAttributesAsync();
                return fn(queue.ApproximateMessageCount);
            });

        public static IVitalityBuilder AddStorageQueueEvaluator(this IVitalityBuilder vitalityBuilder, string component, CloudStorageAccount account, string queueName, Func<int?, bool> fn, TimeSpan cacheAbsoluteExpiration) =>
            vitalityBuilder.AddStorageQueueEvaluator(component, account, async client =>
            {
                var queue = client.GetQueueReference(queueName);
                await queue.FetchAttributesAsync();
                return fn(queue.ApproximateMessageCount);
            }, cacheAbsoluteExpiration);

        public static IVitalityBuilder AddStorageQueueEvaluator(this IVitalityBuilder vitalityBuilder, string component, CloudStorageAccount account, Func<CloudQueueClient, Task<bool>> fn)
        {
            vitalityBuilder.Services.TryAddSingleton<StorageQueueEvaluator>();
            var client = account.CreateCloudQueueClient();
            return vitalityBuilder.AddEvaluator<StorageQueueEvaluator>(component, eval => eval.EvaluateAsync(component, client, fn));
        }

        public static IVitalityBuilder AddStorageQueueEvaluator(this IVitalityBuilder vitalityBuilder, string component, CloudStorageAccount account, Func<CloudQueueClient, Task<bool>> fn, TimeSpan cacheAbsoluteExpiration)
        {
            vitalityBuilder.Services.TryAddSingleton<StorageQueueEvaluator>();
            var client = account.CreateCloudQueueClient();
            return vitalityBuilder.AddEvaluator<StorageQueueEvaluator>(component, cacheAbsoluteExpiration, eval => eval.EvaluateAsync(component, client, fn));
        }

        public static IVitalityBuilder AddStorageTableEvaluator(this IVitalityBuilder vitalityBuilder, string component, CloudStorageAccount account, Func<CloudTableClient, Task<bool>> fn)
        {
            vitalityBuilder.Services.TryAddSingleton<StorageTableEvaluator>();
            var client = account.CreateCloudTableClient();
            return vitalityBuilder.AddEvaluator<StorageTableEvaluator>(component, eval => eval.EvaluateAsync(component, client, fn));
        }

        public static IVitalityBuilder AddStorageTableEvaluator(this IVitalityBuilder vitalityBuilder, string component, CloudStorageAccount account, Func<CloudTableClient, Task<bool>> fn, TimeSpan cacheAbsoluteExpiration)
        {
            vitalityBuilder.Services.TryAddSingleton<StorageTableEvaluator>();
            var client = account.CreateCloudTableClient();
            return vitalityBuilder.AddEvaluator<StorageTableEvaluator>(component, cacheAbsoluteExpiration, eval => eval.EvaluateAsync(component, client, fn));
        }
    }
}
