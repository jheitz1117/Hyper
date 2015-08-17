using System.Runtime.Serialization;
using Hyper.NodeServices.Contracts.Extensibility.CommandModules;

namespace Hyper.NodeServices.Contracts.SystemCommands
{
    /// <summary>
    /// <see cref="ICommandRequest"/> for the <see cref="SystemCommandName.EnableActivityMonitor"/> system command.
    /// </summary>
    [DataContract]
    public class EnableActivityMonitorRequest : ICommandRequest
    {
        /// <summary>
        /// The name of the custom activity monitor to enable or disable.
        /// </summary>
        [DataMember]
        public string ActivityMonitorName { get; set; }

        /// <summary>
        /// Indicates whether to enable or disable the custom activity monitor with the specified name.
        /// </summary>
        [DataMember]
        public bool Enable { get; set; }
    }
}
