using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Hyper.Services.HyperNodeContracts
{
    [DataContract]
    public class HyperNodeMessageResponse
    {
        [DataMember]
        public string TaskId { get; set; }

        [DataMember]
        public string RespondingNodeName { get; set; }

        [DataMember]
        public HyperNodeActionType NodeAction { get; set; }

        [DataMember]
        public HyperNodeActionReasonType NodeActionReason { get; set; }

        [DataMember]
        public MessageProcessStatusFlags ProcessStatusFlags { get; set; }

        [DataMember]
        public List<HyperNodeActivityItem> TaskTrace { get; set; }

        [DataMember]
        public ConcurrentDictionary<string, HyperNodeMessageResponse> ChildResponses { get; set; }

        [DataMember]
        public string CommandResponseString { get; set; }

        public HyperNodeMessageResponse()
        {
            this.TaskTrace = new List<HyperNodeActivityItem>();
            this.ChildResponses = new ConcurrentDictionary<string, HyperNodeMessageResponse>();
        }

        public HyperNodeMessageResponse(string respondingNodeName)
            : this()
        {
            this.RespondingNodeName = respondingNodeName;
        }
    }

    [DataContract]
    public enum HyperNodeActionType
    {
        /// <summary>
        /// Indicates that the receiving <see cref="IHyperNodeService"/> was completely unable to recognize, forward, or otherwise process the message in any capacity.
        /// </summary>
        [EnumMember]
        None = 0,

        /// <summary>
        /// Indicates that the receiving <see cref="IHyperNodeService"/> accepted responsibility for processing the message.
        /// </summary>
        [EnumMember]
        Accepted = 1,

        /// <summary>
        /// Indicates that the receiving <see cref="IHyperNodeService"/> disregarded the message for a valid and expected reason.
        /// </summary>
        [EnumMember]
        Ignored = 2
    }

    [DataContract]
    public enum HyperNodeActionReasonType
    {
        /// <summary>
        /// Indicates that no reason was specified for the action taken by the receiving <see cref="IHyperNodeService"/>.
        /// </summary>
        [EnumMember]
        Unknown = 0,

        /// <summary>
        /// Indicates that the message expired before it could be processed by the receiving <see cref="IHyperNodeService"/>.
        /// </summary>
        [EnumMember]
        MessageExpired = 1,

        /// <summary>
        /// Indicates that the receiving <see cref="IHyperNodeService"/> had already seen the message.
        /// </summary>
        [EnumMember]
        PreviouslySeen = 2,

        /// <summary>
        /// Indicates that the receiving <see cref="IHyperNodeService"/> was not an intended recipient.
        /// </summary>
        [EnumMember]
        UnintendedRecipient = 3,

        /// <summary>
        /// Indicates that the receiving <see cref="IHyperNodeService"/> was an intended recipient.
        /// </summary>
        [EnumMember]
        IntendedRecipient = 4
    }

    /********************************************************************************************************************************************************
     * Using bit-shifts such as 1 << 2 are fast and super-easy to understand. They do powers of 2 for you so you don't have to keep track of them yourself.
     ********************************************************************************************************************************************************/

    [Flags]
    [DataContract]
    public enum MessageProcessStatusFlags
    {
        /// <summary>
        /// Indicates that the message was not processed
        /// </summary>
        [EnumMember]
        None              = 0,

        /// <summary>
        /// Indicates that the message was processed successfully.
        /// </summary>
        [EnumMember]
        Success           = 1 << 0,

        /// <summary>
        /// Indicates that the message could not be processed.
        /// </summary>
        [EnumMember]
        Failure           = 1 << 1,

        /// <summary>
        /// Indicates that the message contained an invalid command name.
        /// </summary>
        [EnumMember]
        InvalidCommand    = 1 << 2,

        /// <summary>
        /// Indicates that some non-fatal errors occurred while processing the message.
        /// </summary>
        [EnumMember]
        HadNonFatalErrors = 1 << 3,

        /// <summary>
        /// Indicates that some warnings occurred while processing the message.
        /// </summary>
        [EnumMember]
        HadWarnings       = 1 << 4,

        /// <summary>
        /// Indicates that the service was cancelled before it completed.
        /// </summary>
        [EnumMember]
        Cancelled         = 1 << 5
    }
}
