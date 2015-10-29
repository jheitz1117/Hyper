using System;
using System.Runtime.Serialization;
using Hyper.NodeServices.Extensibility.Configuration;

namespace Hyper.NodeServices
{
    /// <summary>
    /// Thrown when errors occur while validating an instance of <see cref="IHyperNodeConfiguration"/>.
    /// </summary>
    [Serializable]
    internal class HyperNodeConfigurationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HyperNodeConfigurationException"/> class.
        /// </summary>
        public HyperNodeConfigurationException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="HyperNodeConfigurationException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public HyperNodeConfigurationException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="HyperNodeConfigurationException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
        public HyperNodeConfigurationException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="HyperNodeConfigurationException"/> class with a specified error message
        /// and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public HyperNodeConfigurationException(string message, Exception innerException) : base(message, innerException) { }
    }
}
