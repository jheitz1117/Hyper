using System;
using Hyper.Extensibility.IO;
using Hyper.NodeServices.Contracts.Extensibility.CommandModules;

namespace Hyper.NodeServices.Contracts.Extensibility.Serializers
{
    /// <summary>
    /// Abstract implementation combining <see cref="ICommandRequestSerializer"/> and <see cref="ICommandResponseSerializer"/>
    /// to serialize instances of both <see cref="ICommandRequest"/> and <see cref="ICommandResponse"/>.
    /// </summary>
    /// <typeparam name="TRequest">A type that IS_A <see cref="ICommandRequest"/>.</typeparam>
    /// <typeparam name="TResponse">A type that IS_A <see cref="ICommandResponse"/>.</typeparam>
    public abstract class XmlObjectCommandSerializer<TRequest, TResponse> : ICommandRequestSerializer, ICommandResponseSerializer
        where TRequest : ICommandRequest
        where TResponse : ICommandResponse
    {
        private readonly IStringTransform _requestSerializationTransform;
        private readonly IStringTransform _responseSerializationTransform;

        private XmlObjectRequestSerializer<TRequest> _requestSerializer;
        private XmlObjectRequestSerializer<TRequest> RequestSerializer
        {
            get { return (_requestSerializer ?? (_requestSerializer = CreateRequestSerializer(_requestSerializationTransform))); }
        }

        private XmlObjectResponseSerializer<TResponse> _responseSerializer;
        private XmlObjectResponseSerializer<TResponse> ResponseSerializer
        {
            get { return (_responseSerializer ?? (_responseSerializer = CreateResponseSerializer(_responseSerializationTransform))); }
        }

        /// <summary>
        /// Initializes an instance of <see cref="XmlObjectCommandSerializer{TRequest, TResponse}"/> using <see cref="XmlObjectSerializerWrapper.DefaultStringTransform"/>
        /// for requests and responses.
        /// </summary>
        protected XmlObjectCommandSerializer()
            : this(XmlObjectSerializerWrapper.DefaultStringTransform, XmlObjectSerializerWrapper.DefaultStringTransform) { }

        /// <summary>
        /// Initializes an instance of <see cref="XmlObjectCommandSerializer{TRequest, TResponse}"/> using the specified <see cref="IStringTransform"/>
        /// instances for requests and responses, respectively.
        /// </summary>
        /// <param name="requestSerializationTransform">The <see cref="IStringTransform"/> to use when transforming request data between string and byte representations.</param>
        /// <param name="responseSerializationTransform">The <see cref="IStringTransform"/> to use when transforming response data between string and byte representations.</param>
        protected XmlObjectCommandSerializer(IStringTransform requestSerializationTransform, IStringTransform responseSerializationTransform)
        {
            _requestSerializationTransform = requestSerializationTransform;
            _responseSerializationTransform = responseSerializationTransform;
        }

        /// <summary>
        /// When overridden in a derived class, creates an instance of <see cref="XmlObjectRequestSerializer{T}"/> to use for serialization.
        /// </summary>
        /// <param name="requestSerializationTransform">The <see cref="IStringTransform"/> to use when constructing the <see cref="XmlObjectRequestSerializer{T}"/></param>
        /// <returns></returns>
        protected abstract XmlObjectRequestSerializer<TRequest> CreateRequestSerializer(IStringTransform requestSerializationTransform);
        
        /// <summary>
        /// When overridden in a derived class, creates an instance of <see cref="XmlObjectResponseSerializer{T}"/> to use for serialization.
        /// </summary>
        /// <param name="responseSerializationTransform">The <see cref="IStringTransform"/> to use when constructing the <see cref="XmlObjectResponseSerializer{T}"/></param>
        /// <returns></returns>
        protected abstract XmlObjectResponseSerializer<TResponse> CreateResponseSerializer(IStringTransform responseSerializationTransform);

        /// <summary>
        /// Serializes the specified <see cref="ICommandRequest"/> into a string.
        /// </summary>
        /// <param name="request">The <typeparamref name="TRequest"/> instance to serialize. This instance must implement <see cref="ICommandRequest"/>.</param>
        /// <returns></returns>
        public string Serialize(ICommandRequest request)
        {
            return this.RequestSerializer.Serialize(request);
        }

        /// <summary>
        /// Serializes the specified <see cref="ICommandResponse"/> into a string.
        /// </summary>
        /// <param name="response">The <typeparamref name="TResponse"/> instance to serialize. This instance must implement <see cref="ICommandResponse"/>.</param>
        /// <returns></returns>
        public string Serialize(ICommandResponse response)
        {
            return this.ResponseSerializer.Serialize(response);
        }

        /// <summary>
        /// Deserializes the specified <paramref name="requestString"/> into an object of type <typeparamref name="TRequest" />.
        /// </summary>
        /// <param name="requestString">The request string to deserialize.</param>
        /// <returns></returns>
        ICommandRequest ICommandRequestSerializer.Deserialize(string requestString)
        {
            return ((ICommandRequestSerializer)this.RequestSerializer).Deserialize(requestString);
        }

        /// <summary>
        /// Deserializes the specified <paramref name="responseString"/> into an object of type <typeparamref name="TResponse" />.
        /// </summary>
        /// <param name="responseString">The response string to deserialize.</param>
        /// <returns></returns>
        ICommandResponse ICommandResponseSerializer.Deserialize(string responseString)
        {
            return ((ICommandResponseSerializer)this.ResponseSerializer).Deserialize(responseString);
        }

        /// <summary>
        /// Gets the expected <see cref="Type"/> of the <see cref="ICommandRequest"/> object.
        /// </summary>
        /// <returns></returns>
        public virtual Type GetRequestType()
        {
            return typeof(TRequest);
        }

        /// <summary>
        /// Gets the expected <see cref="Type"/> of the <see cref="ICommandResponse"/> object.
        /// </summary>
        /// <returns></returns>
        public virtual Type GetResponseType()
        {
            return typeof(TResponse);
        }
    }
}
