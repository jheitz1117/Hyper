using System.Runtime.Serialization;
using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Contracts.Extensibility.CommandModules;

namespace NodeModuleTest.Contracts.DbUpdate
{
    [DataContract]
    public class DbUpdateResponse : ICommandResponse
    {
        [DataMember]
        public MessageProcessStatusFlags ProcessStatusFlags { get; set; }
    }
}
