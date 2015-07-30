using Hyper.NodeServices.Contracts.Extensibility;

namespace Hyper.NodeServices.Contracts.Serializers
{
    public sealed class NetDataContractCommandSerializer<TRequest, TResponse> : XmlObjectCommandSerializer<TRequest, TResponse>
        where TRequest : ICommandRequest
        where TResponse : ICommandResponse
    {
        protected override XmlObjectRequestSerializer<TRequest> CreateRequestSerializer()
        {
            return new NetDataContractRequestSerializer<TRequest>();
        }

        protected override XmlObjectResponseSerializer<TResponse> CreateResponseSerializer()
        {
            return new NetDataContractResponseSerializer<TResponse>();
        }
    }
}
