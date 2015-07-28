namespace Hyper.Services.HyperNodeExtensibility
{
    public interface ICommandModule
    {
        ICommandModuleRequestSerializer CreateRequestSerializer();
        ICommandModuleResponseSerializer CreateResponseSerializer();
        ICommandModuleResponse Execute(ICommandExecutionContext context);
    }
}
