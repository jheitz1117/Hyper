using System.Runtime.Serialization;
using Hyper.NodeServices.Contracts.Extensibility;

namespace Hyper.NodeServices.Contracts.Serializers
{
    public sealed class NetDataContractResponseSerializer<T> : XmlObjectResponseSerializer<T> where T : ICommandResponse
    {
        public override XmlObjectSerializer CreateSerializer()
        {
            return new NetDataContractSerializer();
        }
    }
}
