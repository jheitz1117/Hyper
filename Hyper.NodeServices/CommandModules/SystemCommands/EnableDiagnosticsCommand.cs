using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Contracts.Extensibility.CommandModules;
using Hyper.NodeServices.Contracts.Extensibility.Serializers;
using Hyper.NodeServices.Extensibility.CommandModules;
using Hyper.NodeServices.Extensibility.Exceptions;
using Hyper.NodeServices.SystemCommands.Contracts;

namespace Hyper.NodeServices.CommandModules.SystemCommands
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
