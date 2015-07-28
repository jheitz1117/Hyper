using Hyper.Services.HyperNodeContracts;

namespace Hyper.Services.HyperNodeExtensibility
{
    public interface ICommandModuleResponse
    {
        MessageProcessStatusFlags ProcessStatusFlags { get; set; }
    }
}
