using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Hyper.NodeServices.Contracts;

namespace Hyper.NodeServices
{
    /// <summary>
    /// This class provides a read-only version of a <see cref="HyperNodeMessageResponse"/> for use in user-defined code.
    /// </summary>
    internal class ReadOnlyHyperNodeResponseInfo : IReadOnlyHyperNodeResponseInfo
    {
        public string TaskId { get; }
        public string RespondingNodeName { get; }
        public TimeSpan? TotalRunTime { get; }
        public HyperNodeActionType NodeAction { get; }
        public HyperNodeActionReasonType NodeActionReason { get; }
        public MessageProcessStatusFlags ProcessStatusFlags { get; }
        public IReadOnlyList<HyperNodeActivityItem> TaskTrace { get; }
        public IReadOnlyDictionary<string, IReadOnlyHyperNodeResponseInfo> ChildResponses { get; }
        public string CommandResponseString { get; }

        public ReadOnlyHyperNodeResponseInfo(HyperNodeMessageResponse response)
        {
            TaskId = response.TaskId;
            RespondingNodeName = response.RespondingNodeName;
            TotalRunTime = response.TotalRunTime;
            NodeAction = response.NodeAction;
            NodeActionReason = response.NodeActionReason;
            ProcessStatusFlags = response.ProcessStatusFlags;
            CommandResponseString = response.CommandResponseString;
            TaskTrace = new ReadOnlyCollection<HyperNodeActivityItem>(
                response.TaskTrace.Select(
                    t => new HyperNodeActivityItem(t)
                ).ToList()
            );
            ChildResponses = new ReadOnlyDictionary<string, IReadOnlyHyperNodeResponseInfo>(
                response.ChildResponses.Select(
                    kvp => new KeyValuePair<string, IReadOnlyHyperNodeResponseInfo>(
                        kvp.Key,
                        new ReadOnlyHyperNodeResponseInfo(kvp.Value)
                    )
                ).ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
            );
        }
    }
}
