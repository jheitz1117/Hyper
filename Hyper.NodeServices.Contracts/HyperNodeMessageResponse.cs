using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Hyper.NodeServices.Contracts
{
    /// <summary>
    /// The primary response object used by <see cref="IHyperNodeService"/> instances.
    /// </summary>
    [DataContract]
    public class HyperNodeMessageResponse
    {
        /// <summary>
        /// The ID of the task started as a result of the <see cref="HyperNodeMessageRequest"/>. This value
        /// may be null or white space if no task could be started.
        /// </summary>
        [DataMember]
        public string TaskId { get; set; }

        /// <summary>
        /// The name of the <see cref="IHyperNodeService"/> that sent this <see cref="HyperNodeMessageResponse"/>.
        /// </summary>
        [DataMember]
        public string RespondingNodeName { get; set; }

        /// <summary>
        /// If the task completed, contains the total run time of the task.
        /// </summary>
        [DataMember]
        public TimeSpan? TotalRunTime { get; set; }

        /// <summary>
        /// The <see cref="HyperNodeActionType"/> taken by the <see cref="IHyperNodeService"/> in response to the <see cref="HyperNodeMessageRequest"/>.
        /// </summary>
        [DataMember]
        public HyperNodeActionType NodeAction { get; set; }

        /// <summary>
        /// Indicates why the <see cref="IHyperNodeService"/> chose to take the <see cref="HyperNodeActionType"/> reported in the <see cref="NodeAction"/> property.
        /// </summary>
        [DataMember]
        public HyperNodeActionReasonType NodeActionReason { get; set; }

        /// <summary>
        /// A bitwise combination of <see cref="MessageProcessStatusFlags"/> values indicating what happened while the command was running.
        /// </summary>
        [DataMember]
        public MessageProcessStatusFlags ProcessStatusFlags { get; set; }

        /// <summary>
        /// If the <see cref="MessageProcessOptionFlags.ReturnTaskTrace"/> option flag was specified in the <see cref="HyperNodeMessageRequest"/>, contains a list of
        /// <see cref="HyperNodeActivityItem"/> objects tracing the progress of the task up until the point at which this <see cref="HyperNodeMessageResponse"/> was
        /// returned. If the <see cref="MessageProcessOptionFlags.RunConcurrently"/> option flag was specified in the <see cref="HyperNodeMessageRequest"/>, the task
        /// trace will likely be incomplete because the main thread could have completed before the task was finished.
        /// </summary>
        [DataMember]
        public List<HyperNodeActivityItem> TaskTrace { get; set; }

        /// <summary>
        /// Contains a list of <see cref="HyperNodeMessageResponse"/> objects returned by child nodes up until the point at which this
        /// <see cref="HyperNodeMessageResponse"/> was returned. If the <see cref="MessageProcessOptionFlags.RunConcurrently"/> option
        /// flag was specified in the <see cref="HyperNodeMessageRequest"/>, the list will likely be incomplete because the main thread
        /// could have completed before the <see cref="HyperNodeMessageRequest"/> could be forwarded to all child nodes.
        /// </summary>
        [DataMember]
        public ConcurrentDictionary<string, HyperNodeMessageResponse> ChildResponses { get; set; }

        /// <summary>
        /// Contains the response string from the command that was executed.
        /// </summary>
        [DataMember]
        public string CommandResponseString { get; set; }

        /// <summary>
        /// Initializes an instance of <see cref="HyperNodeMessageResponse"/>.
        /// </summary>
        public HyperNodeMessageResponse()
        {
            TaskTrace = new List<HyperNodeActivityItem>();
            ChildResponses = new ConcurrentDictionary<string, HyperNodeMessageResponse>();
        }

        /// <summary>
        /// Initializes an instance of <see cref="HyperNodeMessageResponse"/> using the specified responding node name.
        /// </summary>
        /// <param name="respondingNodeName">The name of the <see cref="IHyperNodeService"/> that created this <see cref="HyperNodeMessageResponse"/>.</param>
        public HyperNodeMessageResponse(string respondingNodeName)
            : this()
        {
            RespondingNodeName = respondingNodeName;
        }
    }
}
