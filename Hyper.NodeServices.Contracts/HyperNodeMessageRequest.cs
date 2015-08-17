using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Hyper.NodeServices.Contracts
{
    /// <summary>
    /// The primary request object used by <see cref="IHyperNodeService"/> instances.
    /// </summary>
    [DataContract]
    public class HyperNodeMessageRequest
    {
        private readonly TimeSpan _defaultMessageLifeSpan = TimeSpan.FromMinutes(1);
        private readonly TimeSpan _defaultForwardingTimeout = TimeSpan.FromSeconds(5);

        /// <summary>
        /// The name of the agent that created this <see cref="HyperNodeMessageRequest"/>.
        /// </summary>
        [DataMember]
        public string CreatedByAgentName { get; set; }

        /// <summary>
        /// The date and time this <see cref="HyperNodeMessageRequest"/> was created.
        /// </summary>
        [DataMember]
        public DateTime CreationDateTime { get; set; }

        /// <summary>
        /// Specifies how long this <see cref="HyperNodeMessageRequest"/> should live before it expires.
        /// </summary>
        [DataMember]
        public TimeSpan MessageLifeSpan { get; set; }

        /// <summary>
        /// A list of <see cref="IHyperNodeService"/> names to which this <see cref="HyperNodeMessageRequest"/> is intended.
        /// </summary>
        [DataMember]
        public List<string> IntendedRecipientNodeNames { get; set; }

        /// <summary>
        /// A list of <see cref="IHyperNodeService"/> names which have already seen this <see cref="HyperNodeMessageRequest"/>.
        /// </summary>
        [DataMember]
        public List<string> SeenByNodeNames { get; set; }

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
        /// Contains the network path this <see cref="HyperNodeMessageRequest"/> should follow in order to arrive at all of the intended recipients.
        /// </summary>
        [DataMember]
        public HyperNodePath ForwardingPath { get; set; }

        /// <summary>
        /// The amount of time to wait before any <see cref="IHyperNodeService"/> gives up on receiving a response from a child node.
        /// </summary>
        [DataMember]
        public TimeSpan ForwardingTimeout { get; set; }

        /// <summary>
        /// A bitwise combination of <see cref="MessageProcessOptionFlags"/> values indicating how this <see cref="HyperNodeMessageRequest"/> should be processed.
        /// </summary>
        [DataMember]
        public MessageProcessOptionFlags ProcessOptionFlags { get; set; }

        /// <summary>
        /// Indicates whether the <see cref="MessageProcessOptionFlags.ReturnTaskTrace"/> option flag was set.
        /// </summary>
        public bool ReturnTaskTrace
        {
            get
            {
                return IsProcessOptionSet(MessageProcessOptionFlags.ReturnTaskTrace);
            }
        }

        /// <summary>
        /// Indicates whether the <see cref="MessageProcessOptionFlags.RunConcurrently"/> option flag was set.
        /// </summary>
        public bool RunConcurrently
        {
            get
            {
                return IsProcessOptionSet(MessageProcessOptionFlags.RunConcurrently);
            }
        }

        /// <summary>
        /// Indicates whether the <see cref="MessageProcessOptionFlags.CacheTaskProgress"/> option flag was set.
        /// </summary>
        public bool CacheTaskProgress
        {
            get
            {
                return IsProcessOptionSet(MessageProcessOptionFlags.CacheTaskProgress);
            }
        }

        /// <summary>
        /// Calculates date and time at which this <see cref="HyperNodeMessageRequest"/> expires.
        /// </summary>
        public DateTime ExpirationDateTime
        {
            get { return this.CreationDateTime + this.MessageLifeSpan; }
        }

        /// <summary>
        /// Initializes an instance of <see cref="HyperNodeMessageRequest"/>.
        /// </summary>
        public HyperNodeMessageRequest()
        {
            this.CreationDateTime = DateTime.Now;
            this.MessageLifeSpan = _defaultMessageLifeSpan;
            this.ForwardingTimeout = _defaultForwardingTimeout;
            this.IntendedRecipientNodeNames = new List<string>();
            this.SeenByNodeNames = new List<string>();
            this.ForwardingPath = new HyperNodePath();
        }

        /// <summary>
        /// Initializes an instance of <see cref="HyperNodeMessageRequest"/> using the specified agent.
        /// </summary>
        /// <param name="createdByAgentName">The agent creating this <see cref="HyperNodeMessageRequest"/>.</param>
        public HyperNodeMessageRequest(string createdByAgentName)
            : this()
        {
            this.CreatedByAgentName = createdByAgentName;
        }

        /// <summary>
        /// Checks whether the specified <see cref="MessageProcessOptionFlags"/> option flag was set.
        /// </summary>
        /// <param name="optionFlag">The <see cref="MessageProcessOptionFlags"/> value to check for.</param>
        /// <returns></returns>
        private bool IsProcessOptionSet(MessageProcessOptionFlags optionFlag)
        {
            return ((this.ProcessOptionFlags & optionFlag) == optionFlag);
        }
    }
}
