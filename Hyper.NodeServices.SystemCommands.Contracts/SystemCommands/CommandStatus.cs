using System.Runtime.Serialization;

namespace Hyper.NodeServices.SystemCommands.Contracts
{
    /// <summary>
    /// Describes the status of a command module.
    /// </summary>
    [DataContract]
    public class CommandStatus
    {
        /// <summary>
        /// The command name of the module.
        /// </summary>
        [DataMember]
        public string CommandName { get; set; }
        
        /// <summary>
        /// Indicates whether the command module is enabled or disabled.
        /// </summary>
        [DataMember]
        public bool Enabled { get; set; }

        /// <summary>
        /// Indicates whether the command module is a <see cref="HyperNodeCommandType.SystemCommand"/> or a <see cref="HyperNodeCommandType.CustomCommand"/>.
        /// </summary>
        [DataMember]
        public HyperNodeCommandType CommandType { get; set; }
    }
}
