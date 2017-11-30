using System.Threading.Tasks;

namespace Vitality
{
    public interface IComponentEvaluator
    {
        string Component { get; }
        Task<ComponentStatus> EvaluateAsync();
    }
}
