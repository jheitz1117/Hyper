using System.Runtime.Serialization;
using Hyper.NodeServices.Contracts.Extensibility.CommandModules;

namespace Hyper.NodeServices.SystemCommands.Contracts
{
    /// <summary>
    /// <see cref="ICommandRequest"/> for the <see cref="SystemCommandName.EnableDiagnostics"/> system command.
    /// </summary>
    [DataContract]
    public class EnableDiagnosticsRequest : ICommandRequest
    {
        /// <summary>
        /// Indicates whether to enable or disable diagnostics.
        /// </summary>
        [DataMember]
        public bool Enable { get; set; }
    }
}
