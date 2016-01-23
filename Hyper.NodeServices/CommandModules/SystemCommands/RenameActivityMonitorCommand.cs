using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Contracts.Extensibility.CommandModules;
using Hyper.NodeServices.Contracts.Extensibility.Serializers;
using Hyper.NodeServices.Extensibility;
using Hyper.NodeServices.Extensibility.CommandModules;
using Hyper.NodeServices.SystemCommands.Contracts;

namespace Hyper.NodeServices.CommandModules.SystemCommands
{
    internal class RenameActivityMonitorCommand : ICommandModule, ICommandRequestSerializerFactory
    {
        public ICommandResponse Execute(ICommandExecutionContext context)
        {
            var request = context.Request as RenameActivityMonitorRequest;
            if (request == null)
                throw new InvalidCommandRequestTypeException(typeof(RenameActivityMonitorRequest), context.Request.GetType());

            var processStatusFlags = MessageProcessStatusFlags.Failure | MessageProcessStatusFlags.InvalidCommandRequest;
            if (HyperNodeService.Instance.IsKnownActivityMonitor(request.OldName))
            {
                var result = HyperNodeService.Instance.RenameActivityMonitor(request.OldName, request.NewName);
                context.Activity.Track($"The activity monitor '{request.OldName}' {(result ? "has been" : "could not be")} renamed to '{request.NewName}'.");

                processStatusFlags = result ? MessageProcessStatusFlags.Success : MessageProcessStatusFlags.Failure;
            }
            else
            {
                context.Activity.Track($"No activity monitor exists with the name '{request.OldName}'.");
            }

            return new CommandResponse(processStatusFlags);
        }

        public ICommandRequestSerializer Create()
        {
            return new NetDataContractRequestSerializer<RenameActivityMonitorRequest>();
        }
    }
}
