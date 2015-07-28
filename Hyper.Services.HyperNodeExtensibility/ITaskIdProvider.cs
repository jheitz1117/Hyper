using Hyper.Services.HyperNodeContracts;

namespace Hyper.Services.HyperNodeExtensibility
{
    public interface ITaskIdProvider
    {
        string CreateTaskId(HyperNodeMessageRequest message);
    }
}
