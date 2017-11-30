using System.Threading.Tasks;

namespace Life
{
    public interface IComponentEvaluator
    {
        string Component { get; }
        Task<ComponentStatus> EvaluateAsync();
    }
}
