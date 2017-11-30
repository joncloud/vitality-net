using System.Collections.Generic;
using System.Threading.Tasks;

namespace Vitality
{
    interface IStatusService
    {
        Task<ComponentStatus> EvaluateComponentAsync(string component);
        Task<IReadOnlyDictionary<string, string>> EvaluateComponentsAsync();
    }
}
