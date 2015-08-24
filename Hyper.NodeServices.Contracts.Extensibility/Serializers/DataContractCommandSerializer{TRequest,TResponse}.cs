using System.Runtime.Serialization;
using Hyper.Extensibility.IO;
using Hyper.NodeServices.Contracts.Extensibility.CommandModules;

namespace Hyper.NodeServices.Contracts.Extensibility.Serializers
{
    /// <summary>
    /// Concrete implementation of <see cref="XmlObjectCommandSerializer{TRequest, TResponse}"/> that can serialize instances
    /// of <see cref="ICommandRequest"/> and <see cref="ICommandResponse"/> using a <see cref="DataContractSerializer"/>.
    /// This class cannot be inherited.
    /// </summary>
    /// <typeparam name="TRequest">A type that IS_A <see cref="ICommandRequest"/>.</typeparam>
    /// <typeparam name="TResponse">A type that IS_A <see cref="ICommandResponse"/>.</typeparam>
    public sealed class DataContractCommandSerializer<TRequest, TResponse> : XmlObjectCommandSerializer<TRequest, TResponse>
        where TRequest : ICommandRequest
        where TResponse : ICommandResponse
    {
        /// <summary>
        /// Initializes an instance of <see cref="DataContractCommandSerializer{TRequest, TResponse}"/> using <see cref="XmlObjectSerializerWrapper.DefaultStringTransform"/>
        /// for requests and responses.
        /// </summary>
        public DataContractCommandSerializer() { }

        /// <summary>
        /// Initializes an instance of <see cref="DataContractCommandSerializer{TRequest, TResponse}"/> using the specified <see cref="IStringTransform"/>
        /// instances for requests and responses, respectively.
        /// </summary>
        /// <param name="requestSerializationTransform">The <see cref="IStringTransform"/> to use when transforming request data between string and byte representations.</param>
        /// <param name="responseSerializationTransform">The <see cref="IStringTransform"/> to use when transforming response data between string and byte representations.</param>
        public DataContractCommandSerializer(IStringTransform requestSerializationTransform, IStringTransform responseSerializationTransform)
            : base(requestSerializationTransform, responseSerializationTransform) { }

        /// <summary>
        /// Creates an instance of <see cref="DataContractRequestSerializer{TRequest}"/> to use for request serialization.
        /// </summary>
        /// <param name="requestSerializationTransform">The <see cref="IStringTransform"/> to use when constructing the <see cref="DataContractRequestSerializer{TRequest}"/></param>
        /// <returns></returns>
        protected override XmlObjectRequestSerializer<TRequest> CreateRequestSerializer(IStringTransform requestSerializationTransform)
        {
            return new DataContractRequestSerializer<TRequest>(requestSerializationTransform);
        }

        /// <summary>
        /// Creates an instance of <see cref="DataContractResponseSerializer{TResponse}"/> to use for response serialization.
        /// </summary>
        /// <param name="responseSerializationTransform">The <see cref="IStringTransform"/> to use when constructing the <see cref="DataContractResponseSerializer{TResponse}"/></param>
        /// <returns></returns>
        protected override XmlObjectResponseSerializer<TResponse> CreateResponseSerializer(IStringTransform responseSerializationTransform)
        {
            return new DataContractResponseSerializer<TResponse>(responseSerializationTransform);
        }
    }
}
