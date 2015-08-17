using System.Runtime.Serialization;
using Hyper.NodeServices.Contracts.Extensibility.CommandModules;

namespace Hyper.NodeServices.Contracts.SystemCommands
{
    /// <summary>
    /// <see cref="ICommandRequest"/> for the <see cref="SystemCommandName.EnableTaskProgressCache"/> system command.
    /// </summary>
    [DataContract]
    public class EnableTaskProgressCacheRequest : ICommandRequest
    {
        /// <summary>
        /// Indicates whether to enable or disable the task progress cache.
        /// </summary>
        [DataMember]
        public bool Enable { get; set; }
    }
}
