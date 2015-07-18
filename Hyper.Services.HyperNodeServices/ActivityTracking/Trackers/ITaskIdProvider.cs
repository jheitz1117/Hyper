using Hyper.Services.HyperNodeContracts;

namespace Hyper.Services.HyperNodeServices
{
    public interface ITaskIdProvider
    {
        string CreateTaskId(HyperNodeMessageRequest message);
    }
}
