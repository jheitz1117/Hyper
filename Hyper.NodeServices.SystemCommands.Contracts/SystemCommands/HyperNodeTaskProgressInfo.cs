using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Hyper.NodeServices.Contracts;

namespace Hyper.NodeServices.SystemCommands.Contracts
{
    /// <summary>
    /// Describes progress information for a task.
    /// </summary>
    [DataContract]
    public class HyperNodeTaskProgressInfo
    {
        /// <summary>
        /// A list of <see cref="HyperNodeActivityItem"/> objects tracing the progress of the task.
        /// </summary>
        [DataMember]
        public List<HyperNodeActivityItem> Activity { get; set; }

        /// <summary>
        /// If the task has completed, contains the <see cref="HyperNodeMessageResponse"/> of the task.
        /// </summary>
        [DataMember]
        public HyperNodeMessageResponse Response { get; set; }

        /// <summary>
        /// If the message was forwarded to one or more child nodes, contains a list of all of the primary task IDs that have been reported to the <see cref="IHyperNodeService"/> at the time of the request.
        /// </summary>
        [DataMember]
        public ConcurrentDictionary<string, string> ChildNodeTaskIds { get; set; }

        /// <summary>
        /// Indicates whether the task has completed.
        /// </summary>
        [DataMember]
        public bool IsComplete { get; set; }

        /// <summary>
        /// The latest partial progress value captured as of the time of the request.
        /// </summary>
        [DataMember]
        public double? ProgressPart { get; set; }

        /// <summary>
        /// The latest total progress value captured as of the time of the request.
        /// </summary>
        [DataMember]
        public double? ProgressTotal { get; set; }

        /// <summary>
        /// Calculates the progress of the task as a percentile by dividing the <see cref="ProgressPart"/> property by the <see cref="ProgressTotal"/> property.
        /// This operation is arithmetically safe.
        /// </summary>
        public double? ProgressPercentage
        {
            get
            {
                if (this.ProgressPart.HasValue && this.ProgressTotal.HasValue && this.ProgressTotal != 0)
                {
                    return this.ProgressPart.Value / this.ProgressTotal.Value;
                }

                return null;
            }
        }

        /// <summary>
        /// Initializes an instance of <see cref="HyperNodeTaskProgressInfo"/>.
        /// </summary>
        public HyperNodeTaskProgressInfo()
        {
            this.Activity = new List<HyperNodeActivityItem>();
            this.ChildNodeTaskIds = new ConcurrentDictionary<string, string>();
        }
    }
}
