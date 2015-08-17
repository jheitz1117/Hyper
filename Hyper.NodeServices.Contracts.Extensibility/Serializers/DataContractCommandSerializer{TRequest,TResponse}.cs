using System.Runtime.Serialization;
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
        /// Creates an instance of <see cref="DataContractRequestSerializer{TRequest}"/> to use for request serialization.
        /// </summary>
        /// <returns></returns>
        protected override XmlObjectRequestSerializer<TRequest> CreateRequestSerializer()
        {
            return new DataContractRequestSerializer<TRequest>();
        }

        /// <summary>
        /// Creates an instance of <see cref="DataContractResponseSerializer{TResponse}"/> to use for response serialization.
        /// </summary>
        /// <returns></returns>
        protected override XmlObjectResponseSerializer<TResponse> CreateResponseSerializer()
        {
            return new DataContractResponseSerializer<TResponse>();
        }
    }
}
