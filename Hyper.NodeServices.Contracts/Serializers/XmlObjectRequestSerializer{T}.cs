using Hyper.NodeServices.Contracts.Extensibility;

namespace Hyper.NodeServices.Contracts.Serializers
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
