using System.Collections.Generic;
using System.Runtime.Serialization;
using Hyper.NodeServices.Contracts.Extensibility;

namespace Hyper.NodeServices.Contracts
{
    // TODO: May need to move this out of the Contracts DLL and into somewhere else. Possibly a whole separate DLL for "stock" request/response objects?
    [DataContract]
    public class HyperNodeProgressInfo : ICommandResponse
    {
        [DataMember]
        public List<HyperNodeActivityItem> Activity { get; set; }

        [DataMember]
        public HyperNodeMessageResponse Response { get; set; }

        [DataMember]
        public bool IsComplete { get; set; }

        [DataMember]
        public double? ProgressPart { get; set; }

        [DataMember]
        public double? ProgressTotal { get; set; }

        public double? ProgressPercentage
        {
            get
            {
                if (this.ProgressPart.HasValue && this.ProgressTotal.HasValue && this.ProgressTotal > 0)
                {
                    return this.ProgressPart.Value / this.ProgressTotal.Value;
                }

                return null;
            }
        }

        public HyperNodeProgressInfo()
        {
            this.Activity = new List<HyperNodeActivityItem>();
        }

        [DataMember]
        public MessageProcessStatusFlags ProcessStatusFlags { get; set; }
    }
}
