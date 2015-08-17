using System;
using System.Runtime.Serialization;
using Hyper.NodeServices.Contracts.Extensibility.CommandModules;

namespace Hyper.NodeServices.Contracts.SystemCommands
{
    /// <summary>
    /// <see cref="ICommandRequest"/> for the <see cref="SystemCommandName.SetTaskProgressCacheDuration"/> system command.
    /// </summary>
    [DataContract]
    public class SetTaskProgressCacheDurationRequest : ICommandRequest
    {
        /// <summary>
        /// The new sliding expiration to set for items in the task progress cache.
        /// </summary>
        [DataMember]
        public TimeSpan CacheDuration { get; set; }
    }
}
