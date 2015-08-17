using System.Runtime.Serialization;

namespace Hyper.NodeServices.Contracts.SystemCommands
{
    [DataContract]
    public enum HyperNodeCommandType
    {
        /// <summary>
        /// Indicates that the command is of unknown origin.
        /// </summary>
        [EnumMember]
        Unknown = 0,

        /// <summary>
        /// Indicates that the command is a built-in system command.
        /// </summary>
        [EnumMember]
        SystemCommand = 1,

        /// <summary>
        /// Indicates that the command is a user-defined command.
        /// </summary>
        [EnumMember]
        CustomCommand = 2
    }
}
