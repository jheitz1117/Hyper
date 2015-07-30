using System.Runtime.Serialization;
using Hyper.Services.HyperNodeContracts.Extensibility;

namespace Hyper.Services.HyperNodeContracts.Serializers
{
    public sealed class NetDataContractResponseSerializer<T> : XmlObjectResponseSerializer<T> where T : ICommandResponse
    {
        public override XmlObjectSerializer CreateSerializer()
        {
            return new NetDataContractSerializer();
        }
    }
}
