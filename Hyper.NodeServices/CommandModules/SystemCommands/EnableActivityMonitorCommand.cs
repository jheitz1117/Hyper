using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Contracts.Extensibility.CommandModules;
using Hyper.NodeServices.Contracts.Extensibility.Serializers;
using Hyper.NodeServices.Contracts.SystemCommands;
using Hyper.NodeServices.Extensibility.CommandModules;
using Hyper.NodeServices.Extensibility.Exceptions;

namespace Hyper.NodeServices.CommandModules.SystemCommands
{
    internal class EnableActivityMonitorCommand : ICommandModule, ICommandRequestSerializerFactory
    {
        public ICommandResponse Execute(ICommandExecutionContext context)
        {
            var request = context.Request as EnableActivityMonitorRequest;
            if (request == null)
                throw new InvalidCommandRequestTypeException(typeof(EnableActivityMonitorRequest), context.Request.GetType());

            var processStatusFlags = MessageProcessStatusFlags.Failure | MessageProcessStatusFlags.InvalidCommandRequest;
            if (HyperNodeService.Instance.IsKnownActivityMonitor(request.ActivityMonitorName))
            {
                var result = HyperNodeService.Instance.EnableActivityMonitor(request.ActivityMonitorName, request.Enable);
                context.Activity.TrackFormat(
                    "The activity monitor '{0}' {1} {2}.",
                    request.ActivityMonitorName,
                    (result ? "is now" : "could not be"),
                    (request.Enable ? "enabled" : "disabled")
                );

                processStatusFlags = (result ? MessageProcessStatusFlags.Success : MessageProcessStatusFlags.Failure);
            }
            else
            {
                context.Activity.TrackFormat("No activity monitor exists with the name '{0}'.", request.ActivityMonitorName);
            }

            return new CommandResponse(processStatusFlags);
        }

        public ICommandRequestSerializer Create()
        {
            return new NetDataContractRequestSerializer<EnableActivityMonitorRequest>();
        }
    }
}
