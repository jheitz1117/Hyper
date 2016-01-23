using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Contracts.Extensibility.CommandModules;
using Hyper.NodeServices.Contracts.Extensibility.Serializers;
using Hyper.NodeServices.Extensibility;
using Hyper.NodeServices.Extensibility.CommandModules;
using Hyper.NodeServices.SystemCommands.Contracts;

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
                context.Activity.Track(
                    $"The activity monitor '{request.ActivityMonitorName}' {(result ? "is now" : "could not be")} {(request.Enable ? "enabled" : "disabled")}."
                );

                processStatusFlags = (result ? MessageProcessStatusFlags.Success : MessageProcessStatusFlags.Failure);
            }
            else
            {
                context.Activity.Track($"No activity monitor exists with the name '{request.ActivityMonitorName}'.");
            }

            return new CommandResponse(processStatusFlags);
        }

        public ICommandRequestSerializer Create()
        {
            return new NetDataContractRequestSerializer<EnableActivityMonitorRequest>();
        }
    }
}
