using Hyper.NodeServices.Contracts;

namespace Hyper.Services.HyperNodeExtensibility
{
    public interface ITaskIdProvider
    {
        string CreateTaskId(HyperNodeMessageRequest message);
    }
}
