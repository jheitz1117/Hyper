using System.Runtime.Serialization;
using Hyper.NodeServices.Contracts.Extensibility.CommandModules;

namespace Hyper.NodeServices.Contracts.SystemCommands
{
    /// <summary>
    /// <see cref="ICommandRequest"/> for the <see cref="SystemCommandName.EnableCommand"/> system command.
    /// </summary>
    [DataContract]
    public class EnableCommandModuleRequest : ICommandRequest
    {
        /// <summary>
        /// The name of the command to enable or disable.
        /// </summary>
        [DataMember]
        public string CommandName { get; set; }

        /// <summary>
        /// Indicates whether to enable or disable the command with the specified command name.
        /// </summary>
        [DataMember]
        public bool Enable { get; set; }
    }
}
