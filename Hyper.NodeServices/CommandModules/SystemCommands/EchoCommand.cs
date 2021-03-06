﻿using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Contracts.Extensibility.CommandModules;
using Hyper.NodeServices.Extensibility;
using Hyper.NodeServices.Extensibility.CommandModules;

namespace Hyper.NodeServices.CommandModules.SystemCommands
{
    internal class EchoCommand : ICommandModule
    {
        public ICommandResponse Execute(ICommandExecutionContext context)
        {
            var request = context.Request as CommandRequestString;
            if (request == null)
                throw new InvalidCommandRequestTypeException(typeof(CommandRequestString), context.Request.GetType());

            var echoString = $"HyperNode '{context.ExecutingNodeName}' says, \"{request.RequestString}\".";
            
            context.Activity.Track(echoString);

            return new CommandResponseString(MessageProcessStatusFlags.Success, echoString);
        }
    }
}
