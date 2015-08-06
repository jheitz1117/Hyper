using System.Runtime.Serialization;

namespace Hyper.NodeServices.Contracts.SystemCommands
{
    [DataContract]
    public class CommandConfiguration
    {
        [DataMember]
        public string CommandName { get; set; }
        
        [DataMember]
        public bool Enabled { get; set; }

        [DataMember]
        public HyperNodeCommandType CommandType { get; set; }
    }
}
