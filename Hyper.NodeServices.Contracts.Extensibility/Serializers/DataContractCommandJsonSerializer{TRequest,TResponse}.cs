using System.Runtime.Serialization.Json;
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
        /// Creates an instance of <see cref="DataContractRequestJsonSerializer{TRequest}"/> to use for request serialization.
        /// </summary>
        /// <returns></returns>
        protected override XmlObjectRequestSerializer<TRequest> CreateRequestSerializer()
        {
            return new DataContractRequestJsonSerializer<TRequest>();
        }

        /// <summary>
        /// Creates an instance of <see cref="DataContractResponseJsonSerializer{TResponse}"/> to use for response serialization.
        /// </summary>
        /// <returns></returns>
        protected override XmlObjectResponseSerializer<TResponse> CreateResponseSerializer()
        {
            return new DataContractResponseJsonSerializer<TResponse>();
        }
    }
}
