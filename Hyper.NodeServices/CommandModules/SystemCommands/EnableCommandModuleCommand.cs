using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Contracts.Extensibility;
using Hyper.NodeServices.Contracts.Serializers;
using Hyper.NodeServices.Contracts.SystemCommands;
using Hyper.NodeServices.Extensibility.CommandModules;
using Hyper.NodeServices.Extensibility.Exceptions;

namespace Hyper.NodeServices.CommandModules.SystemCommands
{
    internal class EnableCommandModuleCommand : ICommandModule, ICommandRequestSerializerFactory
    {
        public ICommandResponse Execute(ICommandExecutionContext context)
        {
            var request = context.Request as EnableCommandModuleRequest;
            if (request == null)
                throw new InvalidCommandRequestTypeException(typeof(EnableCommandModuleRequest), context.Request.GetType());

            var processStatusFlags = MessageProcessStatusFlags.Failure | MessageProcessStatusFlags.InvalidCommandRequest;
            if (request.CommandName == context.CommandName && !request.Enable)
            {
                context.Activity.TrackFormat(
                    "The command '{0}' cannot be disabled remotely. To disable this command, you must modify the configuration.",
                    request.CommandName
                );
            }
            else
            {
                if (HyperNodeService.Instance.IsKnownCommand(request.CommandName ?? ""))
                {
                    var result = HyperNodeService.Instance.EnableCommandModule(request.CommandName, request.Enable);
                    context.Activity.TrackFormat(
                        "The command '{0}' {1} {2}.",
                        request.CommandName,
                        (result ? "is now" : "could not be"),
                        (request.Enable ? "enabled" : "disabled")
                    );

                    processStatusFlags = (result ? MessageProcessStatusFlags.Success : MessageProcessStatusFlags.Failure);
                }
                else
                {
                    context.Activity.TrackFormat("The command '{0}' is invalid.", request.CommandName);
                }
            }

            return new CommandResponse(processStatusFlags);
        }

        public ICommandRequestSerializer Create()
        {
            return new NetDataContractRequestSerializer<EnableCommandModuleRequest>();
        }
    }
}
