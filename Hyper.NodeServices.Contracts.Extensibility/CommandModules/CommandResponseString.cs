namespace Hyper.NodeServices.Contracts.Extensibility.CommandModules
{
    /// <summary>
    /// Wraps a response string from a command module. This class cannot be inherited.
    /// </summary>
    public sealed class CommandResponseString : CommandResponse
    {
        /// <summary>
        /// The response string from the command module.
        /// </summary>
        public string ResponseString { get; set; }

        /// <summary>
        /// Initializes an instance of <see cref="CommandResponseString"/> with the specified <see cref="MessageProcessStatusFlags"/>.
        /// </summary>
        /// <param name="statusFlags">A bitwise combination of <see cref="MessageProcessStatusFlags"/> values indicating what happened while the command was running.</param>
        public CommandResponseString(MessageProcessStatusFlags statusFlags)
            : base(statusFlags) { }

        /// <summary>
        /// Initializes an instance of <see cref="CommandResponseString"/> with the specified <see cref="MessageProcessStatusFlags"/> and <paramref name="responseString"/>.
        /// </summary>
        /// <param name="statusFlags">A bitwise combination of <see cref="MessageProcessStatusFlags"/> values indicating what happened while the command was running.</param>
        /// <param name="responseString">The response string to wrap.</param>
        public CommandResponseString(MessageProcessStatusFlags statusFlags, string responseString)
            : this(statusFlags)
        {
            this.ResponseString = responseString;
        }

        /// <summary>
        /// Returns the value of the <see cref="CommandResponseString.ResponseString"/> property.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.ResponseString;
        }
    }
}
