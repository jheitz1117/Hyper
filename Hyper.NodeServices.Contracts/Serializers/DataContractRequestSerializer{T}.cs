using System.Runtime.Serialization;
using Hyper.NodeServices.Contracts.Extensibility;

namespace Hyper.NodeServices.Contracts.Serializers
{
    public sealed class DataContractRequestSerializer<T> : XmlObjectRequestSerializer<T> where T : ICommandRequest
    {
        public override XmlObjectSerializer CreateSerializer()
        {
            return new DataContractSerializer(typeof(T));
        }
    }
}
