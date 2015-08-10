using System.Runtime.Serialization;

namespace Hyper.NodeServices.Contracts
{
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
        Ignored = 2,

        /// <summary>
        /// Indicates that the receiving <see cref="IHyperNodeService"/> rejected the message because the message would have caused the service to enter an invalid state.
        /// </summary>
        [EnumMember]
        Rejected = 3
    }
}
