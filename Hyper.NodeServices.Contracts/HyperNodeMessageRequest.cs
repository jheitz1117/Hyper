using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Hyper.NodeServices.Contracts
{
    [DataContract]
    public class HyperNodeMessageRequest
    {
        private readonly TimeSpan _defaultMessageLifeSpan = TimeSpan.FromMinutes(1);

        [DataMember]
        public Guid MessageGuid { get; set; }

        [DataMember]
        public string CreatedByAgentName { get; set; }

        [DataMember]
        public DateTime CreationDateTime { get; set; }

        [DataMember]
        public TimeSpan MessageLifeSpan { get; set; }

        public DateTime ExpirationDateTime
        {
            get { return this.CreationDateTime + this.MessageLifeSpan; }
        }

        [DataMember]
        public List<string> IntendedRecipientNodeNames { get; set; }

        [DataMember]
        public List<string> SeenByNodeNames { get; set; }

        [DataMember]
        public string CommandName { get; set; }

        [DataMember]
        public string CommandRequestString { get; set; }

        [DataMember]
        public bool RunConcurrently { get; set; }

        [DataMember]
        public bool ReturnTaskTrace { get; set; }

        [DataMember]
        public bool CacheProgressInfo { get; set; }

        [DataMember]
        public HyperNodePath ForwardingPath { get; set; }

        [DataMember]
        public TimeSpan ForwardingTimeout { get; set; }

        public HyperNodeMessageRequest()
        {
            this.MessageGuid = Guid.NewGuid();
            this.CreationDateTime = DateTime.Now;
            this.MessageLifeSpan = _defaultMessageLifeSpan;
            this.IntendedRecipientNodeNames = new List<string>();
            this.SeenByNodeNames = new List<string>();
            this.ForwardingPath = new HyperNodePath();
        }

        public HyperNodeMessageRequest(string createdByAgentName)
            : this()
        {
            this.CreatedByAgentName = createdByAgentName;
        }
    }
}
