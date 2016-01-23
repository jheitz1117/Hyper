using System.Collections.Concurrent;
using System.Runtime.Serialization;
using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Contracts.Extensibility.CommandModules;

namespace Hyper.NodeServices.SystemCommands.Contracts
{
    /// <summary>
    /// <see cref="ICommandResponse"/> for the <see cref="SystemCommandName.Discover"/> system command.
    /// </summary>
    [DataContract]
    public class DiscoverResponse : ICommandResponse
    {
        /// <summary>
        /// A bitwise combination of <see cref="MessageProcessStatusFlags"/> values indicating what happened while the command was running.
        /// </summary>
        [DataMember]
        public MessageProcessStatusFlags ProcessStatusFlags { get; set; }

        /// <summary>
        /// A dictionary containing the descendants of the <see cref="IHyperNodeService"/>.
        /// </summary>
        [DataMember]
        public ConcurrentDictionary<string, DiscoverResponse> ChildNodes { get; set; } = new ConcurrentDictionary<string, DiscoverResponse>();
    }
}
