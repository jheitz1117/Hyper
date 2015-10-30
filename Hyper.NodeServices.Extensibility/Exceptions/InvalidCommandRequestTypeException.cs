using System;
using System.Runtime.Serialization;
using Hyper.NodeServices.Contracts.Extensibility.CommandModules;
using Hyper.NodeServices.Extensibility.CommandModules;

namespace Hyper.NodeServices.Extensibility
{
    /// <summary>
    /// Thrown when an <see cref="ICommandModule"/> receives an invalid request.
    /// </summary>
    [Serializable]
    public class InvalidCommandRequestTypeException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidCommandRequestTypeException"/> class.
        /// </summary>
        public InvalidCommandRequestTypeException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidCommandRequestTypeException"/> class with a default error message
        /// describing a type conversion error between the expected <see cref="ICommandRequest"/> type and the actual type.
        /// </summary>
        /// <param name="expectedType">The <see cref="Type"/> expected by the <see cref="ICommandModule"/>.</param>
        /// <param name="actualType">The <see cref="Type"/> actually received by the <see cref="ICommandModule"/>.</param>
        public InvalidCommandRequestTypeException(Type expectedType, Type actualType)
            : this(string.Format("Request type '{0}' could not be converted to type '{1}'.", actualType.FullName, expectedType.FullName)) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidCommandRequestTypeException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public InvalidCommandRequestTypeException(string message) : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidCommandRequestTypeException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
        public InvalidCommandRequestTypeException(SerializationInfo info, StreamingContext context) : base(info, context) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidCommandRequestTypeException"/> class with a specified error message
        /// and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public InvalidCommandRequestTypeException(string message, Exception innerException) : base(message, innerException) { }
    }
}
