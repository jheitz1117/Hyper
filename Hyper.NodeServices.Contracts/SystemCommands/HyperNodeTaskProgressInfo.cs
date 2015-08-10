using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Hyper.NodeServices.Contracts.SystemCommands
{
    [DataContract]
    public class HyperNodeTaskProgressInfo
    {
        [DataMember]
        public Guid ParentMessageGuid { get; set; }

        [DataMember]
        public List<HyperNodeActivityItem> Activity { get; set; }

        [DataMember]
        public HyperNodeMessageResponse Response { get; set; }

        [DataMember]
        public ConcurrentDictionary<string, string> ChildTaskIds { get; set; }

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
            this.ChildTaskIds = new ConcurrentDictionary<string, string>();
        }

        public HyperNodeTaskProgressInfo(Guid parentMessageGuid) : this()
        {
            this.ParentMessageGuid = parentMessageGuid;
        }
    }
}
