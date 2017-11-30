using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Vitality
{
    class ServiceProviderComponentEvaluator<T> : IComponentEvaluator
    {
        readonly IServiceProvider _services;
        readonly Func<T, Task<ComponentStatus>> _evaluator;
        public string Component { get; }

        public ServiceProviderComponentEvaluator(string component, IServiceProvider services, Func<T, Task<ComponentStatus>> evaluator)
        {
            Component = component;
            _services = services;
            _evaluator = evaluator;
        }

        public async Task<ComponentStatus> EvaluateAsync()
        {
            using (var scope = _services.CreateScope())
            {
                var service = scope.ServiceProvider.GetRequiredService<T>();
                return await _evaluator(service);
            }
        }
    }

    class ExceptionHandlingComponentEvaluator : IComponentEvaluator
    {
        readonly IComponentEvaluator _inner;
        public string Component => _inner.Component;
        public ExceptionHandlingComponentEvaluator(IComponentEvaluator inner) =>
            _inner = inner;

        public async Task<ComponentStatus> EvaluateAsync()
        {
            try { return await _inner.EvaluateAsync(); }
            catch (Exception ex)
            {
                var details = new Dictionary<string, object>
                {
                    ["Exception"] = ex
                };
                return ComponentStatus.Down(Component, details);
            }
        }
    }

    class LoggingComponentEvaluator : IComponentEvaluator
    {
        private static readonly double TimestampToTicks = TimeSpan.TicksPerSecond / (double)Stopwatch.Frequency;
        readonly IComponentEvaluator _inner;
        readonly ILogger<LoggingComponentEvaluator> _logger;
        public string Component => _inner.Component;

        public LoggingComponentEvaluator(IComponentEvaluator inner, ILogger<LoggingComponentEvaluator> logger)
        {
            _inner = inner;
            _logger = logger;
        }
        public async Task<ComponentStatus> EvaluateAsync()
        {
            var start = Stopwatch.GetTimestamp();
            var status = await _inner.EvaluateAsync();
            var stop = Stopwatch.GetTimestamp();

            var duration = new TimeSpan((long)(TimestampToTicks * (stop - start)));
            _logger.LogInformation(
                "{component} evaluated to {status} in {duration}ms with {details}", 
                Component, status.Status, duration.TotalMilliseconds, status.Details);
            return status;
        }
    }

    class CachingComponentEvaluator : IComponentEvaluator
    {
        readonly IComponentEvaluator _inner;
        readonly IMemoryCache _memoryCache;
        readonly TimeSpan _absoluteExpiration;
        readonly object _key;
        public string Component => _inner.Component;
        public CachingComponentEvaluator(IComponentEvaluator inner, IMemoryCache memoryCache, TimeSpan absoluteExpiration)
        {
            _inner = inner;
            _memoryCache = memoryCache;
            _absoluteExpiration = absoluteExpiration;
            _key = $"CachingComponentEvaluator:{Component}";
        }

        public async Task<ComponentStatus> EvaluateAsync()
        {
            if (!_memoryCache.TryGetValue(_key, out ComponentStatus status))
            {
                status = await _inner.EvaluateAsync();
                _memoryCache.Set(_key, status, _absoluteExpiration);
            }
            return status;
        }
    }

    static class ComponentEvaluatorExtensions
    {
        public static IComponentEvaluator HandleExceptions(this IComponentEvaluator inner)
            => new ExceptionHandlingComponentEvaluator(inner);

        public static IComponentEvaluator Log(this IComponentEvaluator inner, IServiceProvider services)
            => new LoggingComponentEvaluator(inner, services.GetRequiredService<ILogger<LoggingComponentEvaluator>>());

        public static IComponentEvaluator Cache(this IComponentEvaluator inner, IServiceProvider services, TimeSpan absoluteExpiration)
            => new CachingComponentEvaluator(inner, services.GetRequiredService<IMemoryCache>(), absoluteExpiration);
    }
}
