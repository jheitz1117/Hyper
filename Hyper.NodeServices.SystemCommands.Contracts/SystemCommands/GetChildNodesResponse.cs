using System.Collections.Generic;
using System.Runtime.Serialization;
using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Contracts.Extensibility.CommandModules;

namespace Hyper.NodeServices.SystemCommands.Contracts
{
    /// <summary>
    /// <see cref="ICommandResponse"/> for the <see cref="SystemCommandName.GetChildNodes"/> system command.
    /// </summary>
    [DataContract]
    public class GetChildNodesResponse : ICommandResponse
    {
        /// <summary>
        /// A bitwise combination of <see cref="MessageProcessStatusFlags"/> values indicating what happened while the command was running.
        /// </summary>
        [DataMember]
        public MessageProcessStatusFlags ProcessStatusFlags { get; set; }

        /// <summary>
        /// A List of child nodes for the <see cref="IHyperNodeService"/>.
        /// </summary>
        [DataMember]
        public List<string> ChildNodes { get; set; }

        /// <summary>
        /// Initializes an instance of <see cref="GetChildNodesResponse"/>.
        /// </summary>
        public GetChildNodesResponse()
        {
            this.ChildNodes = new List<string>();
        }
    }
}
