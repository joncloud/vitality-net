using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vitality.AzureServiceBus
{
    class NamespaceManagerEvaluator
    {
        public async Task<ComponentStatus> EvaluateSubscriptionAsync(string component, NamespaceManager namespaceManager, string topicPath, string subscriptionName, Func<SubscriptionDescription, bool> fn)
        {
            var subscription = await namespaceManager.GetSubscriptionAsync(topicPath, subscriptionName);

            var details = new Dictionary<string, object>
            {
                ["Uri"] = namespaceManager.Address,
                ["TopicPath"] = topicPath,
                ["SubscriptionName"] = subscriptionName
            };

            return fn(subscription)
                ? ComponentStatus.Up(component, details)
                : ComponentStatus.Down(component, details);

        }

        public async Task<ComponentStatus> EvaluateSubscriptionsAsync(string component, NamespaceManager namespaceManager, string topicPath, Func<IEnumerable<SubscriptionDescription>, bool> fn)
        {
            var subscriptions = await namespaceManager.GetSubscriptionsAsync(topicPath);

            var details = new Dictionary<string, object>
            {
                ["Uri"] = namespaceManager.Address,
                ["TopicPath"] = topicPath
            };

            return fn(subscriptions)
                ? ComponentStatus.Up(component, details)
                : ComponentStatus.Down(component, details);
        }
    }

    public static class NamespaceManagerVitalityBuilderExtensions
    {
        public static IVitalityBuilder AddSubscriptionEvaluator(this IVitalityBuilder vitalityBuilder, string component, NamespaceManager namespaceManager, string topicPath, string subscriptionName, Func<SubscriptionDescription, bool> fn)
        {
            vitalityBuilder.Services.TryAddSingleton<NamespaceManagerEvaluator>();
            return vitalityBuilder.AddEvaluator<NamespaceManagerEvaluator>(component, eval => eval.EvaluateSubscriptionAsync(component, namespaceManager, topicPath, subscriptionName, fn));
        }

        public static IVitalityBuilder AddSubscriptionsEvaluator(this IVitalityBuilder vitalityBuilder, string component, NamespaceManager namespaceManager, string topicPath, Func<IEnumerable<SubscriptionDescription>, bool> fn)
        {
            vitalityBuilder.Services.TryAddSingleton<NamespaceManagerEvaluator>();
            return vitalityBuilder.AddEvaluator<NamespaceManagerEvaluator>(component, eval => eval.EvaluateSubscriptionsAsync(component, namespaceManager, topicPath, fn));
        }
    }
}
