﻿using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Contracts.Extensibility;

namespace Hyper.NodeServices.Extensibility.CommandModules
{
    public class CommandResponse : ICommandResponse
    {
        public MessageProcessStatusFlags ProcessStatusFlags { get; set; }

        public CommandResponse(MessageProcessStatusFlags statusFlags)
        {
            this.ProcessStatusFlags = statusFlags;
        }

        public override string ToString()
        {
            return this.ProcessStatusFlags.ToString();
        }
    }
}