using Hyper.NodeServices;
using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Contracts.Extensibility;
using Hyper.NodeServices.Contracts.Serializers;
using Hyper.NodeServices.Contracts.SystemCommands;
using Hyper.NodeServices.Extensibility.CommandModules;
using Hyper.NodeServices.Extensibility.Exceptions;

namespace HyperNetExtensibilityTest.CommandModules
{
    internal class EnableDiagnosticsCommand : ICommandModule, ICommandRequestSerializerFactory
    {
        public ICommandResponse Execute(ICommandExecutionContext context)
        {
            var request = context.Request as EnableDiagnosticsRequest;
            if (request == null)
                throw new InvalidCommandRequestTypeException(typeof(EnableDiagnosticsRequest), context.Request.GetType());

            HyperNodeService.Instance.EnableDiagnostics = request.Enable;
            context.Activity.TrackFormat(
                "Diagnostics are now {0}.",
                (request.Enable ? "enabled" : "disabled")
            );

            return new CommandResponse(MessageProcessStatusFlags.Success);
        }

        public ICommandRequestSerializer Create()
        {
            return new NetDataContractRequestSerializer<EnableDiagnosticsRequest>();
        }
    }
}
