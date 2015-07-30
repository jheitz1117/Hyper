using Hyper.NodeServices.Contracts;

namespace Hyper.NodeServices.Extensibility
{
    public interface ITaskIdProvider
    {
        void Initialize();
        string CreateTaskId(HyperNodeMessageRequest message);
    }
}
