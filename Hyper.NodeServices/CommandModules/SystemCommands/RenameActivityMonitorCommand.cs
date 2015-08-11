﻿using Hyper.NodeServices;
using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Contracts.Extensibility;
using Hyper.NodeServices.Contracts.Serializers;
using Hyper.NodeServices.Contracts.SystemCommands;
using Hyper.NodeServices.Extensibility.CommandModules;
using Hyper.NodeServices.Extensibility.Exceptions;

namespace HyperNetExtensibilityTest.CommandModules
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
                context.Activity.TrackFormat(
                    "The activity monitor '{0}' {1} renamed to '{2}'.",
                    request.OldName,
                    (result ? "has been" : "could not be"),
                    request.NewName
                );

                processStatusFlags = (result ? MessageProcessStatusFlags.Success : MessageProcessStatusFlags.Failure);
            }
            else
            {
                context.Activity.TrackFormat("No activity monitor exists with the name '{0}'.", request.OldName);
            }

            return new CommandResponse(processStatusFlags);
        }

        public ICommandRequestSerializer Create()
        {
            return new NetDataContractRequestSerializer<RenameActivityMonitorRequest>();
        }
    }
}