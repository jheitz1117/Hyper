﻿using System;
using System.Runtime.Serialization;
using Hyper.NodeServices.Contracts.Extensibility;

namespace Hyper.NodeServices.Contracts.SystemCommands
{
    [DataContract]
    public class SetTaskProgressCacheDurationRequest : ICommandRequest
    {
        [DataMember]
        public TimeSpan CacheDuration { get; set; }
    }
}