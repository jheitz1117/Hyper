using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Hyper.NodeServices.Contracts.Extensibility;

namespace Hyper.NodeServices.Contracts.SystemCommands
{
    [DataContract]
    public class GetNodeStatusResponse : ICommandResponse
    {
        [DataMember]
        public MessageProcessStatusFlags ProcessStatusFlags { get; set; }

        [DataMember]
        public TimeSpan TaskProgressCacheDuration { get; set; }

        [DataMember]
        public bool TaskProgressCacheEnabled { get; set; }

        [DataMember]
        public bool DiagnosticsEnabled { get; set; }

        [DataMember]
        public int MaxConcurrentTasks { get; set; }

        [DataMember]
        public IEnumerable<CommandStatus> Commands { get; set; }

        [DataMember]
        public IEnumerable<ActivityMonitorStatus> ActivityMonitors { get; set; }

        [DataMember]
        public IEnumerable<LiveTaskStatus> LiveTasks { get; set; }

        public GetNodeStatusResponse()
        {
            this.Commands = new List<CommandStatus>();
            this.ActivityMonitors = new List<ActivityMonitorStatus>();
            this.LiveTasks = new List<LiveTaskStatus>();
        }
    }
}
