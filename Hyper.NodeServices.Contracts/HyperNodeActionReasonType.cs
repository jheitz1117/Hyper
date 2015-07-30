using System.Runtime.Serialization;

namespace Hyper.NodeServices.Contracts
{
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
}
