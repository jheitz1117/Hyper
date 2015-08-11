using System;
using Hyper.NodeServices.Contracts.Extensibility;

namespace Hyper.NodeServices.Contracts.Serializers
{
    public abstract class XmlObjectCommandSerializer<TRequest, TResponse> : ICommandRequestSerializer, ICommandResponseSerializer
        where TRequest : ICommandRequest
        where TResponse : ICommandResponse
    {
        private XmlObjectRequestSerializer<TRequest> _requestSerializer;
        private XmlObjectRequestSerializer<TRequest> RequestSerializer
        {
            get { return (_requestSerializer ?? (_requestSerializer = CreateRequestSerializer())); }
        }

        private XmlObjectResponseSerializer<TResponse> _responseSerializer;
        private XmlObjectResponseSerializer<TResponse> ResponseSerializer
        {
            get { return (_responseSerializer ?? (_responseSerializer = CreateResponseSerializer())); }
        }

        protected abstract XmlObjectRequestSerializer<TRequest> CreateRequestSerializer();
        protected abstract XmlObjectResponseSerializer<TResponse> CreateResponseSerializer();

        public string Serialize(ICommandRequest request)
        {
            return this.RequestSerializer.Serialize(request);
        }
        
        public string Serialize(ICommandResponse response)
        {
            return this.ResponseSerializer.Serialize(response);
        }

        ICommandRequest ICommandRequestSerializer.Deserialize(string requestString)
        {
            return ((ICommandRequestSerializer)this.RequestSerializer).Deserialize(requestString);
        }

        ICommandResponse ICommandResponseSerializer.Deserialize(string responseString)
        {
            return ((ICommandResponseSerializer)this.ResponseSerializer).Deserialize(responseString);
        }

        public virtual Type GetRequestType()
        {
            return typeof(TRequest);
        }

        public virtual Type GetResponseType()
        {
            return typeof(TResponse);
        }
    }
}
