using System.Threading.Tasks;

namespace Lyfe
{
    public interface IComponentEvaluator
    {
        string Component { get; }
        Task<ComponentStatus> EvaluateAsync();
    }
}
