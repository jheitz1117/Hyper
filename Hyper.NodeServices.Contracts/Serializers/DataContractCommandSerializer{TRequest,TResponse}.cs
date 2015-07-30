using Hyper.NodeServices.Contracts.Extensibility;

namespace Hyper.NodeServices.Contracts.Serializers
{
    public sealed class DataContractCommandSerializer<TRequest, TResponse> : XmlObjectCommandSerializer<TRequest, TResponse>
        where TRequest : ICommandRequest
        where TResponse : ICommandResponse
    {
        protected override XmlObjectRequestSerializer<TRequest> CreateRequestSerializer()
        {
            return new DataContractRequestSerializer<TRequest>();
        }

        protected override XmlObjectResponseSerializer<TResponse> CreateResponseSerializer()
        {
            return new DataContractResponseSerializer<TResponse>();
        }
    }
}
