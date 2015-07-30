using Hyper.Services.HyperNodeContracts.Extensibility;
using Hyper.Services.HyperNodeContracts.Serializers;
using Hyper.Services.HyperNodeExtensibility;

namespace HyperNetExtensibilityTest.CommandModules
{
    public class DataContractCombinedSerializerFactory<TRequest, TResponse> : ICommandRequestSerializerFactory, ICommandResponseSerializerFactory
        where TRequest : ICommandRequest
        where TResponse : ICommandResponse
    {
        private object _serializer;
        private object Serializer
        {
            get { return _serializer ?? (_serializer = new DataContractCommandSerializer<TRequest, TResponse>()); }
        }

        ICommandRequestSerializer ICommandRequestSerializerFactory.Create()
        {
            return this.Serializer as ICommandRequestSerializer;
        }

        ICommandResponseSerializer ICommandResponseSerializerFactory.Create()
        {
            return this.Serializer as ICommandResponseSerializer;
        }
    }

    public class DataContractRequestSerializerFactory<TRequest> : ICommandRequestSerializerFactory
        where TRequest : ICommandRequest
    {
        private ICommandRequestSerializer _serializer;
        private ICommandRequestSerializer Serializer
        {
            get { return _serializer ?? (_serializer = new DataContractRequestSerializer<TRequest>()); }
        }

        public ICommandRequestSerializer Create()
        {
            return this.Serializer;
        }
    }

    public class DataContractResponseSerializerFactory<TResponse> : ICommandResponseSerializerFactory
        where TResponse : ICommandResponse
    {
        private ICommandResponseSerializer _serializer;
        private ICommandResponseSerializer Serializer
        {
            get { return _serializer ?? (_serializer = new DataContractResponseSerializer<TResponse>()); }
        }

        public ICommandResponseSerializer Create()
        {
            return this.Serializer;
        }
    }
}
