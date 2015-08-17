using Hyper.NodeServices.Contracts.Extensibility;

namespace Hyper.NodeServices.Extensibility.CommandModules
{
    /// <summary>
    /// Wraps a request string to pass to an <see cref="ICommandModule"/>. This class cannot be inherited.
    /// </summary>
    public sealed class CommandRequestString : ICommandRequest
    {
        /// <summary>
        /// The request string to pass to the <see cref="ICommandModule"/>.
        /// </summary>
        public string RequestString { get; set; }

        /// <summary>
        /// Initializes an instance of <see cref="CommandRequestString"/> using the specified <paramref name="requestString"/>.
        /// </summary>
        /// <param name="requestString">The request string to wrap.</param>
        public CommandRequestString(string requestString)
        {
            this.RequestString = requestString;
        }

        /// <summary>
        /// Returns the value of the <see cref="CommandRequestString.RequestString"/> property.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.RequestString;
        }
    }
}
