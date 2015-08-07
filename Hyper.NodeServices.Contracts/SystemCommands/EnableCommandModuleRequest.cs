﻿using System.Runtime.Serialization;
using Hyper.NodeServices.Contracts.Extensibility;

namespace Hyper.NodeServices.Contracts.SystemCommands
{
    [DataContract]
    public class EnableCommandModuleRequest : ICommandRequest
    {
        [DataMember]
        public string CommandName { get; set; }

        [DataMember]
        public bool Enable { get; set; }
    }
}
