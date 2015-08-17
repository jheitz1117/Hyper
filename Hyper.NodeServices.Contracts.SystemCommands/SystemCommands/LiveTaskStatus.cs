using System;
using System.Runtime.Serialization;

namespace Hyper.NodeServices.Contracts.SystemCommands
{
    /// <summary>
    /// Describes the status of a task.
    /// </summary>
    [DataContract]
    public class LiveTaskStatus
    {
        /// <summary>
        /// The ID of the task.
        /// </summary>
        [DataMember]
        public string TaskID { get; set; }

        /// <summary>
        /// The name of the command which started the task.
        /// </summary>
        [DataMember]
        public string CommandName { get; set; }

        /// <summary>
        /// Indicates whether cancellation has been requested for the task.
        /// </summary>
        [DataMember]
        public bool IsCancellationRequested { get; set; }

        /// <summary>
        /// Indicates how long the task has been running.
        /// </summary>
        [DataMember]
        public TimeSpan Elapsed { get; set; }
    }
}
