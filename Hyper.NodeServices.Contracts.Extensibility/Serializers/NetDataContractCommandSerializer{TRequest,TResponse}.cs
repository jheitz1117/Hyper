using System.Runtime.Serialization;
using Hyper.Extensibility.IO;
using Hyper.NodeServices.Contracts.Extensibility.CommandModules;

namespace Hyper.NodeServices.Contracts.Extensibility.Serializers
{
    /// <summary>
    /// Concrete implementation of <see cref="XmlObjectCommandSerializer{TRequest, TResponse}"/> that can serialize instances
    /// of <see cref="ICommandRequest"/> and <see cref="ICommandResponse"/> using a <see cref="NetDataContractSerializer"/>.
    /// This class cannot be inherited.
    /// </summary>
    /// <typeparam name="TRequest">A type that IS_A <see cref="ICommandRequest"/>.</typeparam>
    /// <typeparam name="TResponse">A type that IS_A <see cref="ICommandResponse"/>.</typeparam>
    public sealed class NetDataContractCommandSerializer<TRequest, TResponse> : XmlObjectCommandSerializer<TRequest, TResponse>
        where TRequest : ICommandRequest
        where TResponse : ICommandResponse
    {
        /// <summary>
        /// Initializes an instance of <see cref="NetDataContractCommandSerializer{TRequest, TResponse}"/> using <see cref="XmlObjectSerializerWrapper.DefaultStringTransform"/>
        /// for requests and responses.
        /// </summary>
        public NetDataContractCommandSerializer() { }

        /// <summary>
        /// Initializes an instance of <see cref="NetDataContractCommandSerializer{TRequest, TResponse}"/> using the specified <see cref="IStringTransform"/>
        /// instances for requests and responses, respectively.
        /// </summary>
        /// <param name="requestSerializationTransform">The <see cref="IStringTransform"/> to use when transforming request data between string and byte representations.</param>
        /// <param name="responseSerializationTransform">The <see cref="IStringTransform"/> to use when transforming response data between string and byte representations.</param>
        public NetDataContractCommandSerializer(IStringTransform requestSerializationTransform, IStringTransform responseSerializationTransform)
            : base(requestSerializationTransform, responseSerializationTransform) { }

        /// <summary>
        /// Creates an instance of <see cref="NetDataContractRequestSerializer{TRequest}"/> to use for request serialization.
        /// </summary>
        /// <param name="requestSerializationTransform">The <see cref="IStringTransform"/> to use when constructing the <see cref="NetDataContractRequestSerializer{TRequest}"/></param>
        /// <returns></returns>
        protected override XmlObjectRequestSerializer<TRequest> CreateRequestSerializer(IStringTransform requestSerializationTransform)
        {
            return new NetDataContractRequestSerializer<TRequest>(requestSerializationTransform);
        }

        /// <summary>
        /// Creates an instance of <see cref="NetDataContractResponseSerializer{TResponse}"/> to use for response serialization.
        /// </summary>
        /// <param name="responseSerializationTransform">The <see cref="IStringTransform"/> to use when constructing the <see cref="NetDataContractResponseSerializer{TResponse}"/></param>
        /// <returns></returns>
        protected override XmlObjectResponseSerializer<TResponse> CreateResponseSerializer(IStringTransform responseSerializationTransform)
        {
            return new NetDataContractResponseSerializer<TResponse>(responseSerializationTransform);
        }
    }
}
