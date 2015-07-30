using Hyper.NodeServices.Contracts.Extensibility;

namespace Hyper.Services.HyperNodeExtensibility
{
    public interface ICommandModule
    {
        ICommandResponse Execute(ICommandExecutionContext context);
    }
}
