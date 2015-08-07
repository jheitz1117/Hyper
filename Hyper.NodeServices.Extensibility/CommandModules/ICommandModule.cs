using Hyper.NodeServices.Contracts.Extensibility;

namespace Hyper.NodeServices.Extensibility.CommandModules
{
    public interface ICommandModule
    {
        ICommandResponse Execute(ICommandExecutionContext context);
    }
}
