using System;
using System.Collections.Generic;

namespace Hyper.NodeServices.Contracts
{
    /// <summary>
    /// Contains information from <see cref="HyperNodeMessageResponse"/> objects received from remote <see cref="IHyperNodeService"/> instances.
    /// </summary>
    public interface IReadOnlyHyperNodeResponseInfo
    {
        /// <summary>
        /// The ID of the task started as a result of the <see cref="HyperNodeMessageRequest"/>. This value
        /// may be null or white space if no task could be started.
        /// </summary>
        string TaskId { get; }

        /// <summary>
        /// The name of the <see cref="IHyperNodeService"/> that sent this <see cref="HyperNodeMessageResponse"/>.
        /// </summary>
        string RespondingNodeName { get; }

        /// <summary>
        /// If the task completed, contains the total run time of the task.
        /// </summary>
        TimeSpan? TotalRunTime { get; }

        /// <summary>
        /// The <see cref="HyperNodeActionType"/> taken by the <see cref="IHyperNodeService"/> in response to the <see cref="HyperNodeMessageRequest"/>.
        /// </summary>
        HyperNodeActionType NodeAction { get; }

        /// <summary>
        /// Indicates why the <see cref="IHyperNodeService"/> chose to take the <see cref="HyperNodeActionType"/> reported in the <see cref="NodeAction"/> property.
        /// </summary>
        HyperNodeActionReasonType NodeActionReason { get; }

        /// <summary>
        /// A bitwise combination of <see cref="MessageProcessStatusFlags"/> values indicating what happened while the command was running.
        /// </summary>
        MessageProcessStatusFlags ProcessStatusFlags { get; }

        /// <summary>
        /// If the <see cref="MessageProcessOptionFlags.ReturnTaskTrace"/> option flag was specified in the <see cref="HyperNodeMessageRequest"/>, contains a list of
        /// <see cref="HyperNodeActivityItem"/> objects tracing the progress of the task up until the point at which this <see cref="HyperNodeMessageResponse"/> was
        /// returned. If the <see cref="MessageProcessOptionFlags.RunConcurrently"/> option flag was specified in the <see cref="HyperNodeMessageRequest"/>, the task
        /// trace will likely be incomplete because the main thread could have completed before the task was finished.
        /// </summary>
        IReadOnlyList<HyperNodeActivityItem> TaskTrace { get; }

        /// <summary>
        /// Contains the response string from the command that was executed.
        /// </summary>
        string CommandResponseString { get; }
    }
}
