using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using Hyper.NodeServices.Contracts.Extensibility;

namespace Hyper.NodeServices.Contracts.Serializers
{
    public sealed class DataContractResponseJsonSerializer<T> : XmlObjectResponseSerializer<T> where T : ICommandResponse
    {
        public override XmlObjectSerializer CreateSerializer()
        {
            return new DataContractJsonSerializer(typeof(T));
        }
    }
}
