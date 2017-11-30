using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Threading.Tasks;

namespace Vitality
{
    public interface IVitalityBuilder
    {
        PathString Path { get; set; }
        IServiceCollection Services { get; }
        Func<HttpContext, Task<bool>> AuthorizeDetails { get; set; }
        JsonSerializerSettings JsonSettings { get; set; }

        IVitalityBuilder AddEvaluator<T>(string component, Func<T, Task<ComponentStatus>> evaluator);
        IVitalityBuilder AddEvaluator<T>(string component, TimeSpan cacheAbsoluteExpiration, Func<T, Task<ComponentStatus>> evaluator);
        IVitalityBuilder AddEvaluator<T>() where T : class, IComponentEvaluator;
        IVitalityBuilder AddEvaluator<T>(TimeSpan cacheAbsoluteExpiration) where T : class, IComponentEvaluator;
    }

    class VitalityBuilder : IVitalityBuilder
    {
        public PathString Path { get; set; } = "/vitality";
        public IServiceCollection Services { get; }
        public Func<HttpContext, Task<bool>> AuthorizeDetails { get; set; } = _ => Task.FromResult(false);
        public JsonSerializerSettings JsonSettings { get; set; }

        public VitalityBuilder(IServiceCollection services)
        {
            Services = services;
            var contractResolver = new CamelCasePropertyNamesContractResolver();
            JsonSettings = new JsonSerializerSettings
            {
                ContractResolver = contractResolver,
                NullValueHandling = NullValueHandling.Ignore
            };
        }

        internal void Configure(VitalityOptions options)
        {
            options.Path = Path;
            options.NotUpStatusCode = StatusCodes.Status200OK;
            options.UpStatusCode = StatusCodes.Status200OK;
            options.AuthorizeDetails = AuthorizeDetails;
            options.JsonSettings = JsonSettings;
        }

        static IComponentEvaluator FromFunc<T>(string component, IServiceProvider services, Func<T, Task<ComponentStatus>> evaluator) =>
            new ServiceProviderComponentEvaluator<T>(component, services, evaluator);

        public IVitalityBuilder AddEvaluator<T>(string component, Func<T, Task<ComponentStatus>> evaluator)
        {
            Services.AddSingleton(svc => Wrap(FromFunc(component, svc, evaluator), svc));
            return this;
        }

        public IVitalityBuilder AddEvaluator<T>(string component, TimeSpan cacheAbsoluteExpiration, Func<T, Task<ComponentStatus>> evaluator)
        {
            Services.AddSingleton(svc => Wrap(FromFunc(component, svc, evaluator), svc, cacheAbsoluteExpiration));
            return this;
        }

        public IVitalityBuilder AddEvaluator<T>() where T : class, IComponentEvaluator
        {
            Services.AddSingleton<T>();
            Services.AddSingleton(svc => Wrap(svc.GetRequiredService<T>(), svc));
            return this;
        }

        public IVitalityBuilder AddEvaluator<T>(TimeSpan cacheAbsoluteExpiration) where T : class, IComponentEvaluator
        {
            Services.AddSingleton<T>();
            Services.AddSingleton(svc => Wrap(svc.GetRequiredService<T>(), svc, cacheAbsoluteExpiration));
            return this;
        }

        static IComponentEvaluator Wrap(IComponentEvaluator evaluator, IServiceProvider services)
            => evaluator.HandleExceptions().Log(services);
        static IComponentEvaluator Wrap(IComponentEvaluator evaluator, IServiceProvider services, TimeSpan cacheAbsoluteExpiration)
            => evaluator.HandleExceptions().Cache(services, cacheAbsoluteExpiration).Log(services);
    }
}
