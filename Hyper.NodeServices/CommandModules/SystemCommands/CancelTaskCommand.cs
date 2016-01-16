﻿using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Contracts.Extensibility.CommandModules;
using Hyper.NodeServices.Extensibility;
using Hyper.NodeServices.Extensibility.CommandModules;

namespace Hyper.NodeServices.CommandModules.SystemCommands
{
    internal class CancelTaskCommand : ICommandModule
    {
        public ICommandResponse Execute(ICommandExecutionContext context)
        {
            var request = context.Request as CommandRequestString;
            if (request == null)
                throw new InvalidCommandRequestTypeException(typeof(CommandRequestString), context.Request.GetType());

            var result = HyperNodeService.Instance.CancelTask(request.RequestString);
            context.Activity.TrackFormat(
                "The task with ID '{0}' {1} cancelled.",
                request.RequestString,
                (result ? "was" : "could not be")
            );

            return new CommandResponse((result ? MessageProcessStatusFlags.Success : MessageProcessStatusFlags.Failure));
        }
    }
}
