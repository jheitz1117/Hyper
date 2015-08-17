using System;

namespace Hyper.NodeServices.Contracts.Extensibility
{
    /// <summary>
    /// Provides methods to serialize and deserialize <see cref="ICommandRequest"/> objects.
    /// </summary>
    public interface ICommandRequestSerializer
    {
        /// <summary>
        /// Serializes the specified <see cref="ICommandRequest"/> object into a string.
        /// </summary>
        /// <param name="request">The <see cref="ICommandRequest"/> object to serialize.</param>
        /// <returns></returns>
        string Serialize(ICommandRequest request);

        /// <summary>
        /// Deserializes the specified <paramref name="requestString"/> and returns an <see cref="ICommandRequest"/> object.
        /// </summary>
        /// <param name="requestString">The string containing the object data to deserialize.</param>
        /// <returns></returns>
        ICommandRequest Deserialize(string requestString);

        /// <summary>
        /// Gets the expected <see cref="Type"/> of the <see cref="ICommandRequest"/> object.
        /// </summary>
        /// <returns></returns>
        Type GetRequestType();
    }
}
