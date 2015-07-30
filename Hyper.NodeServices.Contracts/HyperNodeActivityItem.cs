using System;
using System.Runtime.Serialization;

namespace Hyper.NodeServices.Contracts
{
    [DataContract]
    public class HyperNodeActivityItem
    {
        [DataMember]
        public DateTime EventDateTime { get; set; }

        [DataMember]
        public string Agent { get; set; }

        [DataMember]
        public string EventDescription { get; set; }

        [DataMember]
        public string EventDetail { get; set; }

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

        public HyperNodeActivityItem()
        {
            this.EventDateTime = DateTime.Now;
        }

        public HyperNodeActivityItem(string agent)
            : this()
        {
            this.Agent = agent;
        }
    }
}
