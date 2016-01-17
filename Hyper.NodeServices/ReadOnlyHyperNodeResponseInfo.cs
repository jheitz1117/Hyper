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
        public string TaskId { get; set; }
        public string RespondingNodeName { get; set; }
        public TimeSpan? TotalRunTime { get; set; }
        public HyperNodeActionType NodeAction { get; set; }
        public HyperNodeActionReasonType NodeActionReason { get; set; }
        public MessageProcessStatusFlags ProcessStatusFlags { get; set; }
        public IReadOnlyList<HyperNodeActivityItem> TaskTrace { get; set; }
        public IReadOnlyDictionary<string, IReadOnlyHyperNodeResponseInfo> ChildResponses { get; set; }
        public string CommandResponseString { get; set; }

        public ReadOnlyHyperNodeResponseInfo(IEnumerable<HyperNodeActivityItem> taskTrace, IDictionary<string, HyperNodeMessageResponse> childResponses)
        {
            // Copy in the info from our top-level message and response. We're avoiding assignment so that
            // if the user changes anything, it doesn't affect the top-level response
            this.TaskTrace = new ReadOnlyCollection<HyperNodeActivityItem>(
                taskTrace.Select(
                    t => new HyperNodeActivityItem
                    {
                        Agent = t.Agent,
                        Elapsed = t.Elapsed,
                        EventDateTime = t.EventDateTime,
                        EventDescription = t.EventDescription,
                        EventDetail = t.EventDetail,
                        ProgressPart = t.ProgressPart,
                        ProgressTotal = t.ProgressTotal
                    }
                ).ToList()
            );
            
            this.ChildResponses = new ReadOnlyDictionary<string, IReadOnlyHyperNodeResponseInfo>(
                childResponses.Select(
                    kvp => new KeyValuePair<string, IReadOnlyHyperNodeResponseInfo>(
                        kvp.Key,
                        new ReadOnlyHyperNodeResponseInfo(kvp.Value.TaskTrace, kvp.Value.ChildResponses)
                        {
                            TaskId = kvp.Value.TaskId,
                            CommandResponseString = kvp.Value.CommandResponseString,
                            NodeAction = kvp.Value.NodeAction,
                            NodeActionReason = kvp.Value.NodeActionReason,
                            ProcessStatusFlags = kvp.Value.ProcessStatusFlags,
                            RespondingNodeName = kvp.Value.RespondingNodeName,
                            TotalRunTime = kvp.Value.TotalRunTime
                        }
                    )
                ).ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
            );
        }
    }
}
