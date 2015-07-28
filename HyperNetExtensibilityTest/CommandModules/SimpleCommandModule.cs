using Hyper.Services.HyperNodeExtensibility;

namespace HyperNetExtensibilityTest.CommandModules
{
    public abstract class SimpleCommandModule : ICommandModule
    {
        public ICommandModuleRequestSerializer CreateRequestSerializer()
        {
            return null;
        }

        public ICommandModuleResponseSerializer CreateResponseSerializer()
        {
            return null;
        }

        public abstract ICommandModuleResponse Execute(ICommandExecutionContext context);
    }
}
