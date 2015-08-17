using System;

namespace Hyper.NodeServices.Contracts.Extensibility.CommandModules
{
    /// <summary>
    /// Provides methods to serialize and deserialize <see cref="ICommandResponse"/> objects.
    /// </summary>
    public interface ICommandResponseSerializer
    {
        /// <summary>
        /// Serializes the specified <see cref="ICommandResponse"/> object into a string.
        /// </summary>
        /// <param name="response">The <see cref="ICommandResponse"/> object to serialize.</param>
        /// <returns></returns>
        string Serialize(ICommandResponse response);

        /// <summary>
        /// Deserializes the specified <paramref name="responseString"/> and returns an <see cref="ICommandResponse"/> object.
        /// </summary>
        /// <param name="responseString">The string containing the object data to deserialize.</param>
        /// <returns></returns>
        ICommandResponse Deserialize(string responseString);

        /// <summary>
        /// Gets the expected <see cref="Type"/> of the <see cref="ICommandResponse"/> object.
        /// </summary>
        /// <returns></returns>
        Type GetResponseType();
    }
}
