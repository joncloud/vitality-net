using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Threading.Tasks;

namespace Lyfe
{
    public interface ILyfeBuilder
    {
        PathString Path { get; set; }
        IServiceCollection Services { get; }
        Func<HttpContext, Task<bool>> AuthorizeDetails { get; set; }
        JsonSerializerSettings JsonSettings { get; set; }

        ILyfeBuilder AddEvaluator<T>(string component, Func<T, Task<ComponentStatus>> evaluator);
        ILyfeBuilder AddEvaluator<T>(string component, TimeSpan cacheAbsoluteExpiration, Func<T, Task<ComponentStatus>> evaluator);
        ILyfeBuilder AddEvaluator<T>() where T : class, IComponentEvaluator;
        ILyfeBuilder AddEvaluator<T>(TimeSpan cacheAbsoluteExpiration) where T : class, IComponentEvaluator;
    }

    class LyfeBuilder : ILyfeBuilder
    {
        public PathString Path { get; set; } = "/lyfe";
        public IServiceCollection Services { get; }
        public Func<HttpContext, Task<bool>> AuthorizeDetails { get; set; } = _ => Task.FromResult(false);
        public JsonSerializerSettings JsonSettings { get; set; }

        public LyfeBuilder(IServiceCollection services)
        {
            Services = services;
            var contractResolver = new CamelCasePropertyNamesContractResolver();
            JsonSettings = new JsonSerializerSettings
            {
                ContractResolver = contractResolver,
                NullValueHandling = NullValueHandling.Ignore
            };
        }

        internal void Configure(LyfeOptions options)
        {
            options.Path = Path;
            options.NotUpStatusCode = StatusCodes.Status200OK;
            options.UpStatusCode = StatusCodes.Status200OK;
            options.AuthorizeDetails = AuthorizeDetails;
            options.JsonSettings = JsonSettings;
        }

        static IComponentEvaluator FromFunc<T>(string component, IServiceProvider services, Func<T, Task<ComponentStatus>> evaluator) =>
            new ServiceProviderComponentEvaluator<T>(component, services, evaluator);

        public ILyfeBuilder AddEvaluator<T>(string component, Func<T, Task<ComponentStatus>> evaluator)
        {
            Services.AddSingleton(svc => Wrap(FromFunc(component, svc, evaluator), svc));
            return this;
        }

        public ILyfeBuilder AddEvaluator<T>(string component, TimeSpan cacheAbsoluteExpiration, Func<T, Task<ComponentStatus>> evaluator)
        {
            Services.AddSingleton(svc => Wrap(FromFunc(component, svc, evaluator), svc, cacheAbsoluteExpiration));
            return this;
        }

        public ILyfeBuilder AddEvaluator<T>() where T : class, IComponentEvaluator
        {
            Services.AddSingleton<T>();
            Services.AddSingleton(svc => Wrap(svc.GetRequiredService<T>(), svc));
            return this;
        }

        public ILyfeBuilder AddEvaluator<T>(TimeSpan cacheAbsoluteExpiration) where T : class, IComponentEvaluator
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
