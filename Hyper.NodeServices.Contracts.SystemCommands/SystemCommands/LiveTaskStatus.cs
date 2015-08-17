using System;
using System.Runtime.Serialization;

namespace Hyper.NodeServices.Contracts.SystemCommands
{
    [DataContract]
    public class LiveTaskStatus
    {
        [DataMember]
        public string TaskID { get; set; }

        [DataMember]
        public string CommandName { get; set; }

        [DataMember]
        public bool IsCancellationRequested { get; set; }

        [DataMember]
        public TimeSpan Elapsed { get; set; }
    }
}
