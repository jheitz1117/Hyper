using System.Runtime.Serialization;
using Hyper.NodeServices.Contracts.Extensibility.CommandModules;

namespace Hyper.NodeServices.Contracts.SystemCommands
{
    /// <summary>
    /// <see cref="ICommandResponse"/> for the <see cref="SystemCommandName.GetCachedTaskProgressInfo"/> system command.
    /// </summary>
    [DataContract]
    public class GetCachedTaskProgressInfoResponse : ICommandResponse
    {
        /// <summary>
        /// A bitwise combination of <see cref="MessageProcessStatusFlags"/> values indicating what happened while the command was running.
        /// </summary>
        [DataMember]
        public MessageProcessStatusFlags ProcessStatusFlags { get; set; }

        /// <summary>
        /// Indicates whether the task progress cache is enabled for the <see cref="IHyperNodeService"/>.
        /// </summary>
        [DataMember]
        public bool TaskProgressCacheEnabled { get; set; }

        /// <summary>
        /// If the cache is enabled, contains the progress of the task with the requested ID.
        /// </summary>
        [DataMember]
        public HyperNodeTaskProgressInfo TaskProgressInfo { get; set; }
    }
}
