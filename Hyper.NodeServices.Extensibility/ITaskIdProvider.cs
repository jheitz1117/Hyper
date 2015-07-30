using Hyper.NodeServices.Contracts;

namespace Hyper.NodeServices.Extensibility
{
    public interface ITaskIdProvider
    {
        string CreateTaskId(HyperNodeMessageRequest message);
    }
}
