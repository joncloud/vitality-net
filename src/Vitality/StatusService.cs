using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Vitality
{
    class StatusService : IStatusService
    {
        readonly Dictionary<string, IComponentEvaluator> _evaluators;
        public StatusService(IEnumerable<IComponentEvaluator> evaluators) =>
            _evaluators = evaluators.ToDictionary(eval => eval.Component);

        public async Task<ComponentStatus> EvaluateComponentAsync(string component)
        {
            if (_evaluators.TryGetValue(component, out var evaluator))
                return await evaluator.EvaluateAsync();

            return ComponentStatus.Down(component);
        }

        public async Task<IReadOnlyDictionary<string, string>> EvaluateComponentsAsync()
        {
            var tasks = _evaluators.Values.Select(eval => eval.EvaluateAsync());

            var statuses = await Task.WhenAll(tasks);

            return statuses.ToDictionary(status => status.Component, status => status.Status);
        }
    }
}
