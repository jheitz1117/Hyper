using System;

namespace Hyper.NodeServices.Contracts.Extensibility.CommandModules
{
    /// <summary>
    /// Wraps a bitwise combination of <see cref="MessageProcessStatusFlags"/> values indicating what happened while the command was running.
    /// This class is the default <see cref="ICommandResponse"/> type for all command modules if no other <see cref="ICommandResponse"/> is
    /// specified.
    /// </summary>
    public class CommandResponse : ICommandResponse
    {
        /// <summary>
        /// A bitwise combination of <see cref="MessageProcessStatusFlags"/> values indicating what happened while the command was running.
        /// </summary>
        public MessageProcessStatusFlags ProcessStatusFlags { get; set; }

        /// <summary>
        /// Initializes an instance of <see cref="CommandResponse"/> with the specified <see cref="MessageProcessStatusFlags"/>.
        /// </summary>
        /// <param name="statusFlags">A bitwise combination of <see cref="MessageProcessStatusFlags"/> values indicating what happened while the command was running.</param>
        public CommandResponse(MessageProcessStatusFlags statusFlags)
        {
            ProcessStatusFlags = statusFlags;
        }

        /// <summary>
        /// Calls the <see cref="Enum"/>.<see cref="Enum.ToString()"/> method on the <see cref="ProcessStatusFlags"/> property and returns the results.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ProcessStatusFlags.ToString();
        }
    }
}
