using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Life
{
    public interface ILifeBuilder
    {
        PathString Path { get; set; }
        IServiceCollection Services { get; }

        ILifeBuilder AddEvaluator<T>(string component, Func<T, Task<ComponentStatus>> evaluator);
        ILifeBuilder AddEvaluator<T>(string component, TimeSpan cacheAbsoluteExpiration, Func<T, Task<ComponentStatus>> evaluator);
        ILifeBuilder AddEvaluator<T>() where T : class, IComponentEvaluator;
        ILifeBuilder AddEvaluator<T>(TimeSpan cacheAbsoluteExpiration) where T : class, IComponentEvaluator;
    }

    class LifeBuilder : ILifeBuilder
    {
        public PathString Path { get; set; } = "/life";
        public IServiceCollection Services { get; }

        public LifeBuilder(IServiceCollection services) =>
            Services = services;

        internal void Configure(LifeOptions options)
        {
            options.Path = Path;
            options.NotUpStatusCode = StatusCodes.Status200OK;
            options.UpStatusCode = StatusCodes.Status200OK;
        }

        static IComponentEvaluator FromFunc<T>(string component, IServiceProvider services, Func<T, Task<ComponentStatus>> evaluator) =>
            new ServiceProviderComponentEvaluator<T>(component, services, evaluator);

        public ILifeBuilder AddEvaluator<T>(string component, Func<T, Task<ComponentStatus>> evaluator)
        {
            Services.AddSingleton(svc => Wrap(FromFunc(component, svc, evaluator), svc));
            return this;
        }

        public ILifeBuilder AddEvaluator<T>(string component, TimeSpan cacheAbsoluteExpiration, Func<T, Task<ComponentStatus>> evaluator)
        {
            Services.AddSingleton(svc => Wrap(FromFunc(component, svc, evaluator), svc, cacheAbsoluteExpiration));
            return this;
        }

        public ILifeBuilder AddEvaluator<T>() where T : class, IComponentEvaluator
        {
            Services.AddSingleton<T>();
            Services.AddSingleton(svc => Wrap(svc.GetRequiredService<T>(), svc));
            return this;
        }

        public ILifeBuilder AddEvaluator<T>(TimeSpan cacheAbsoluteExpiration) where T : class, IComponentEvaluator
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
