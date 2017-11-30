using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Vitality.Tests
{
    public class DetailsTests : IntegrationTests
    {
        [Fact]
        public Task ShouldReturnComponentDetails() =>
            TestAsync(UseStatusProvider, async http =>
            {
                var json = await http.GetStringAsync("/vitality/Status");
                var status = JsonConvert.DeserializeObject<ComponentStatus>(json);

                var expected = StatusProvider.Details;
                var actual = status.Details;
                
                Assert.Equal(expected.Keys, actual.Keys);
                Assert.Equal(expected.Values, actual.Values);
            });

        [Fact]
        public Task ShouldReturnExceptionContents() =>
            TestAsync(UseExceptionThrower, async http =>
            {
                var json = await http.GetStringAsync("/vitality/ExceptionThrower");
                var status = JsonConvert.DeserializeObject<ComponentStatus>(json);

                Assert.Contains("exception", status.Details.Keys);
                var jobject = Assert.IsType<JObject>(status.Details["exception"]);
                string actual = jobject.Property("Message").Value.Value<string>();
                Assert.Equal("Exception thrown", actual);
            });

        static IVitalityBuilder AuthorizeAll(IVitalityBuilder options)
        {
            options.AuthorizeDetails = _ => Task.FromResult(true);
            return options;
        }

        static void UseStatusProvider(IVitalityBuilder options) =>
            AuthorizeAll(options).AddEvaluator<StatusProvider>().Services.AddSingleton<StatusProvider>();

        class StatusProvider : IComponentEvaluator
        {
            public string Component { get; } = "Status";

            public static IReadOnlyDictionary<string, object> Details { get; } =
                new Dictionary<string, object>
                {
                    ["string"] = "abc",
                    ["long"] = 123L
                };

            public Task<ComponentStatus> EvaluateAsync()
            {
                var componentStatus = ComponentStatus.Up(Component, Details);
                return Task.FromResult(componentStatus);
            }
        }

        static void UseExceptionThrower(IVitalityBuilder options) =>
            AuthorizeAll(options).AddEvaluator<ExceptionThrower>().Services.AddSingleton<ExceptionThrower>();

        class ExceptionThrower : IComponentEvaluator
        {
            public string Component { get; } = "ExceptionThrower";

            public Task<ComponentStatus> EvaluateAsync() =>
                throw new Exception("Exception thrown");
        }
    }
}
