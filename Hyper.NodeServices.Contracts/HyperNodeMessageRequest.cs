using System.Runtime.Serialization;

namespace Hyper.NodeServices.Contracts
{
    /// <summary>
    /// The primary request object used by <see cref="IHyperNodeService"/> instances.
    /// </summary>
    [DataContract]
    public class HyperNodeMessageRequest
    {
        /// <summary>
        /// The name of the agent that created this <see cref="HyperNodeMessageRequest"/>.
        /// </summary>
        [DataMember]
        public string CreatedByAgentName { get; set; }

        /// <summary>
        /// The name of the command to execute.
        /// </summary>
        [DataMember]
        public string CommandName { get; set; }

        /// <summary>
        /// A string containing the request parameters for the command to execute.
        /// </summary>
        [DataMember]
        public string CommandRequestString { get; set; }

        /// <summary>
        /// A bitwise combination of <see cref="MessageProcessOptionFlags"/> values indicating how this <see cref="HyperNodeMessageRequest"/> should be processed.
        /// </summary>
        [DataMember]
        public MessageProcessOptionFlags ProcessOptionFlags { get; set; }

        /// <summary>
        /// Indicates whether the <see cref="MessageProcessOptionFlags.ReturnTaskTrace"/> option flag was set.
        /// </summary>
        public bool ReturnTaskTrace => IsProcessOptionSet(MessageProcessOptionFlags.ReturnTaskTrace);

        /// <summary>
        /// Indicates whether the <see cref="MessageProcessOptionFlags.RunConcurrently"/> option flag was set.
        /// </summary>
        public bool RunConcurrently => IsProcessOptionSet(MessageProcessOptionFlags.RunConcurrently);

        /// <summary>
        /// Indicates whether the <see cref="MessageProcessOptionFlags.CacheTaskProgress"/> option flag was set.
        /// </summary>
        public bool CacheTaskProgress => IsProcessOptionSet(MessageProcessOptionFlags.CacheTaskProgress);

        /// <summary>
        /// Initializes an instance of <see cref="HyperNodeMessageRequest"/>.
        /// </summary>
        public HyperNodeMessageRequest() { }

        /// <summary>
        /// Initializes an instance of <see cref="HyperNodeMessageRequest"/> using the specified agent.
        /// </summary>
        /// <param name="createdByAgentName">The agent creating this <see cref="HyperNodeMessageRequest"/>.</param>
        public HyperNodeMessageRequest(string createdByAgentName)
            : this()
        {
            CreatedByAgentName = createdByAgentName;
        }

        /// <summary>
        /// Checks whether the specified <see cref="MessageProcessOptionFlags"/> option flag was set.
        /// </summary>
        /// <param name="optionFlag">The <see cref="MessageProcessOptionFlags"/> value to check for.</param>
        /// <returns></returns>
        private bool IsProcessOptionSet(MessageProcessOptionFlags optionFlag)
        {
            return (ProcessOptionFlags & optionFlag) == optionFlag;
        }
    }
}
