using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Hyper.NodeServices.Contracts
{
    [DataContract]
    public class HyperNodeMessageResponse
    {
        [DataMember]
        public string TaskId { get; set; }

        [DataMember]
        public string RespondingNodeName { get; set; }

        [DataMember]
        public HyperNodeActionType NodeAction { get; set; }

        [DataMember]
        public HyperNodeActionReasonType NodeActionReason { get; set; }

        [DataMember]
        public MessageProcessStatusFlags ProcessStatusFlags { get; set; }

        [DataMember]
        public List<HyperNodeActivityItem> TaskTrace { get; set; }

        [DataMember]
        public ConcurrentDictionary<string, HyperNodeMessageResponse> ChildResponses { get; set; }

        [DataMember]
        public string CommandResponseString { get; set; }

        public HyperNodeMessageResponse()
        {
            this.TaskTrace = new List<HyperNodeActivityItem>();
            this.ChildResponses = new ConcurrentDictionary<string, HyperNodeMessageResponse>();
        }

        public HyperNodeMessageResponse(string respondingNodeName)
            : this()
        {
            this.RespondingNodeName = respondingNodeName;
        }
    }
}
