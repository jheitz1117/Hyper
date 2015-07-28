using Hyper.Services.HyperNodeExtensibility;
using HyperNet.ExtensibilityTest.Shared.CommandModules;

namespace HyperNetExtensibilityTest.CommandModules
{
    public abstract class ComplexCommandModuleBase<TRequest, TResponse> : ICommandModule
        where TRequest : ICommandModuleRequest
        where TResponse : ICommandModuleResponse
    {
        public ICommandModuleRequestSerializer CreateRequestSerializer()
        {
            return new DataContractCommandSerializer<TRequest, TResponse>();
        }

        public ICommandModuleResponseSerializer CreateResponseSerializer()
        {
            return new DataContractCommandSerializer<TRequest, TResponse>();
        }

        public abstract ICommandModuleResponse Execute(ICommandExecutionContext context);
    }
}
