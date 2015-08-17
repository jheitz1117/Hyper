using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Hyper.NodeServices.Contracts.Extensibility.CommandModules;

namespace Hyper.NodeServices.Contracts.SystemCommands
{
    /// <summary>
    /// <see cref="ICommandResponse"/> for the <see cref="SystemCommandName.GetNodeStatus"/> system command.
    /// </summary>
    [DataContract]
    public class GetNodeStatusResponse : ICommandResponse
    {
        /// <summary>
        /// A bitwise combination of <see cref="MessageProcessStatusFlags"/> values indicating what happened while the command was running.
        /// </summary>
        [DataMember]
        public MessageProcessStatusFlags ProcessStatusFlags { get; set; }

        /// <summary>
        /// The sliding expiration of items in the task progress cache.
        /// </summary>
        [DataMember]
        public TimeSpan TaskProgressCacheDuration { get; set; }

        /// <summary>
        /// Indicates whether the task progress cache is enabled or disabled.
        /// </summary>
        [DataMember]
        public bool TaskProgressCacheEnabled { get; set; }

        /// <summary>
        /// Indicates whether diagnostics are enabled or disabled.
        /// </summary>
        [DataMember]
        public bool DiagnosticsEnabled { get; set; }

        /// <summary>
        /// The maximum number of tasks that are allowed to run concurrently.
        /// </summary>
        [DataMember]
        public int MaxConcurrentTasks { get; set; }

        /// <summary>
        /// A list of <see cref="CommandStatus"/> objects reporting the status of each command module in the <see cref="IHyperNodeService"/>.
        /// </summary>
        [DataMember]
        public IEnumerable<CommandStatus> Commands { get; set; }

        /// <summary>
        /// A list of <see cref="ActivityMonitorStatus"/> objects reporting the status of each custom activity monitor in the <see cref="IHyperNodeService"/>.
        /// </summary>
        [DataMember]
        public IEnumerable<ActivityMonitorStatus> ActivityMonitors { get; set; }

        /// <summary>
        /// A list of <see cref="LiveTaskStatus"/> objects reporting the status of each live task in the <see cref="IHyperNodeService"/>.
        /// </summary>
        [DataMember]
        public IEnumerable<LiveTaskStatus> LiveTasks { get; set; }

        /// <summary>
        /// Initializes an instance of <see cref="GetNodeStatusResponse"/>.
        /// </summary>
        public GetNodeStatusResponse()
        {
            this.Commands = new List<CommandStatus>();
            this.ActivityMonitors = new List<ActivityMonitorStatus>();
            this.LiveTasks = new List<LiveTaskStatus>();
        }
    }
}
