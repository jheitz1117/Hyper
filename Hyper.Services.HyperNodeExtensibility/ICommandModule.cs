namespace Hyper.Services.HyperNodeExtensibility
{
    public interface ICommandModule
    {
        ICommandModuleResponse Execute(ICommandExecutionContext context);
    }
}
