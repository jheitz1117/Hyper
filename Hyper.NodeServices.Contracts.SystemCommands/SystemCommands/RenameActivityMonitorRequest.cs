using System.Runtime.Serialization;
using Hyper.NodeServices.Contracts.Extensibility.CommandModules;

namespace Hyper.NodeServices.Contracts.SystemCommands
{
    [DataContract]
    public class RenameActivityMonitorRequest : ICommandRequest
    {
        [DataMember]
        public string OldName { get; set; }

        [DataMember]
        public string NewName { get; set; }
    }
}
