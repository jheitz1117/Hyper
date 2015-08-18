using System.Runtime.Serialization;
using Hyper.NodeServices.Contracts.Extensibility.CommandModules;

namespace Hyper.NodeServices.SystemCommands.Contracts
{
    /// <summary>
    /// <see cref="ICommandRequest"/> for the <see cref="SystemCommandName.RenameActivityMonitor"/> system command.
    /// </summary>
    [DataContract]
    public class RenameActivityMonitorRequest : ICommandRequest
    {
        /// <summary>
        /// The name of the custom activity monitor to rename.
        /// </summary>
        [DataMember]
        public string OldName { get; set; }

        /// <summary>
        /// The new name of the custom activity monitor.
        /// </summary>
        [DataMember]
        public string NewName { get; set; }
    }
}
