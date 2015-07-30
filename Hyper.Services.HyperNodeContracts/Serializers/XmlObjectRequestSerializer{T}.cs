using Hyper.Services.HyperNodeContracts.Extensibility;

namespace Hyper.Services.HyperNodeContracts.Serializers
{
    public abstract class XmlObjectRequestSerializer<T> : XmlObjectSerializerWrapper<T>, ICommandRequestSerializer
        where T : ICommandRequest
    {
        public string Serialize(ICommandRequest request)
        {
            return base.Serialize((T)request);
        }

        ICommandRequest ICommandRequestSerializer.Deserialize(string requestString)
        {
            return base.Deserialize(requestString);
        }
    }
}
