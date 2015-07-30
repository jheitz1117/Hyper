using System.Runtime.Serialization;
using Hyper.NodeServices.Contracts.Extensibility;

namespace Hyper.NodeServices.Contracts.Serializers
{
    public sealed class DataContractResponseSerializer<T> : XmlObjectResponseSerializer<T> where T : ICommandResponse
    {
        public override XmlObjectSerializer CreateSerializer()
        {
            return new DataContractSerializer(typeof(T));
        }
    }
}
