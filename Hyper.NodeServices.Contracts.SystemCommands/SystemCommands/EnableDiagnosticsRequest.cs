using System.Runtime.Serialization;
using Hyper.NodeServices.Contracts.Extensibility.CommandModules;

namespace Hyper.NodeServices.Contracts.SystemCommands
{
    [DataContract]
    public class EnableDiagnosticsRequest : ICommandRequest
    {
        [DataMember]
        public bool Enable { get; set; }
    }
}
