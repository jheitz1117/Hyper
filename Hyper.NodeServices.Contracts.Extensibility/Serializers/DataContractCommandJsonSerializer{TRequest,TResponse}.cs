using System.Runtime.Serialization.Json;
using Hyper.Extensibility.IO;
using Hyper.NodeServices.Contracts.Extensibility.CommandModules;

namespace Hyper.NodeServices.Contracts.Extensibility.Serializers
{
    /// <summary>
    /// Concrete implementation of <see cref="XmlObjectCommandSerializer{TRequest, TResponse}"/> that can serialize instances
    /// of <see cref="ICommandRequest"/> and <see cref="ICommandResponse"/> using a <see cref="DataContractJsonSerializer"/>.
    /// This class cannot be inherited.
    /// </summary>
    /// <typeparam name="TRequest">A type that IS_A <see cref="ICommandRequest"/>.</typeparam>
    /// <typeparam name="TResponse">A type that IS_A <see cref="ICommandResponse"/>.</typeparam>
    public sealed class DataContractCommandJsonSerializer<TRequest, TResponse> : XmlObjectCommandSerializer<TRequest, TResponse>
        where TRequest : ICommandRequest
        where TResponse : ICommandResponse
    {
        /// <summary>
        /// Initializes an instance of <see cref="DataContractCommandJsonSerializer{TRequest, TResponse}"/> using <see cref="XmlObjectSerializerWrapper.DefaultStringTransform"/>
        /// for requests and responses.
        /// </summary>
        public DataContractCommandJsonSerializer() { }

        /// <summary>
        /// Initializes an instance of <see cref="DataContractCommandJsonSerializer{TRequest, TResponse}"/> using the specified <see cref="IStringTransform"/>
        /// instances for requests and responses, respectively.
        /// </summary>
        /// <param name="requestSerializationTransform">The <see cref="IStringTransform"/> to use when transforming request data between string and byte representations.</param>
        /// <param name="responseSerializationTransform">The <see cref="IStringTransform"/> to use when transforming response data between string and byte representations.</param>
        public DataContractCommandJsonSerializer(IStringTransform requestSerializationTransform, IStringTransform responseSerializationTransform)
            : base(requestSerializationTransform, responseSerializationTransform) { }

        /// <summary>
        /// Creates an instance of <see cref="DataContractRequestJsonSerializer{TRequest}"/> to use for request serialization.
        /// </summary>
        /// <param name="requestSerializationTransform">The <see cref="IStringTransform"/> to use when constructing the <see cref="DataContractRequestJsonSerializer{TRequest}"/></param>
        /// <returns></returns>
        protected override XmlObjectRequestSerializer<TRequest> CreateRequestSerializer(IStringTransform requestSerializationTransform)
        {
            return new DataContractRequestJsonSerializer<TRequest>(requestSerializationTransform);
        }

        /// <summary>
        /// Creates an instance of <see cref="DataContractResponseJsonSerializer{TResponse}"/> to use for response serialization.
        /// </summary>
        /// <param name="responseSerializationTransform">The <see cref="IStringTransform"/> to use when constructing the <see cref="DataContractResponseJsonSerializer{TResponse}"/></param>
        /// <returns></returns>
        protected override XmlObjectResponseSerializer<TResponse> CreateResponseSerializer(IStringTransform responseSerializationTransform)
        {
            return new DataContractResponseJsonSerializer<TResponse>(responseSerializationTransform);
        }
    }
}
