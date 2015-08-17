using System.Runtime.Serialization;

namespace Hyper.NodeServices.Contracts.SystemCommands
{
    [DataContract]
    public class ActivityMonitorStatus
    {
        [DataMember]
        public string Name { get; set; }
        
        [DataMember]
        public bool Enabled { get; set; }
    }
}
