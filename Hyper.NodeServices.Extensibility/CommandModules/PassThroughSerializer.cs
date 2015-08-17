using System;
using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Contracts.Extensibility;

namespace Hyper.NodeServices.Extensibility.CommandModules
{
    /// <summary>
    /// Passes strings back and forth without performing any serialization. This is the default implementation for both the <see cref="ICommandRequestSerializer"/> and <see cref="ICommandResponseSerializer"/> interfaces.
    /// </summary>
    public sealed class PassThroughSerializer : ICommandRequestSerializer, ICommandResponseSerializer
    {
        /// <summary>
        /// Returns the result of the <see cref="ICommandRequest"/>.<see cref="object.ToString()"/> method.
        /// </summary>
        /// <param name="request">The <see cref="ICommandRequest"/> object to serialize.</param>
        /// <returns></returns>
        public string Serialize(ICommandRequest request)
        {
            return request.ToString();
        }

        /// <summary>
        /// Returns the result of the <see cref="ICommandResponse"/>.<see cref="object.ToString()"/> method.
        /// </summary>
        /// <param name="response">The <see cref="ICommandResponse"/> object to serialize.</param>
        /// <returns></returns>
        public string Serialize(ICommandResponse response)
        {
            return response.ToString();
        }

        /// <summary>
        /// Returns a <see cref="CommandRequestString"/> containing the specified <paramref name="requestString"/>.
        /// </summary>
        /// <param name="requestString">The request string to wrap.</param>
        /// <returns></returns>
        ICommandRequest ICommandRequestSerializer.Deserialize(string requestString)
        {
            return new CommandRequestString(requestString);
        }

        /// <summary>
        /// Returns a <see cref="CommandResponseString"/> containing the specified <paramref name="responseString"/>.
        /// </summary>
        /// <param name="responseString">The response string to wrap.</param>
        /// <returns></returns>
        ICommandResponse ICommandResponseSerializer.Deserialize(string responseString)
        {
            return new CommandResponseString(MessageProcessStatusFlags.None, responseString);
        }

        /// <summary>
        /// The expected <see cref="ICommandRequest"/> type for a <see cref="PassThroughSerializer"/> is <see cref="CommandRequestString"/>.
        /// </summary>
        /// <returns></returns>
        public Type GetRequestType()
        {
            return typeof(CommandRequestString);
        }

        /// <summary>
        /// The expected <see cref="ICommandResponse"/> type for a <see cref="PassThroughSerializer"/> is <see cref="CommandResponseString"/>.
        /// </summary>
        /// <returns></returns>
        public Type GetResponseType()
        {
            return typeof(CommandResponseString);
        }
    }
}
