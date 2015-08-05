using System.Collections.Generic;
using System.Runtime.Serialization;
using Hyper.NodeServices.Contracts.Extensibility;

namespace Hyper.NodeServices.Contracts.SystemCommands
{
    [DataContract]
    public class HyperNodeTaskProgressInfo : ICommandResponse
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

        public HyperNodeTaskProgressInfo()
        {
            this.Activity = new List<HyperNodeActivityItem>();
        }

        [DataMember]
        public MessageProcessStatusFlags ProcessStatusFlags { get; set; }
    }
}
