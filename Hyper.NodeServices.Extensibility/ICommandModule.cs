using Hyper.NodeServices.Contracts.Extensibility;

namespace Hyper.NodeServices.Extensibility
{
    public interface ICommandModule
    {
        ICommandResponse Execute(ICommandExecutionContext context);
    }
}
