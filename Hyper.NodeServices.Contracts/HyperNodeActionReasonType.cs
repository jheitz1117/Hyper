using System.Runtime.Serialization;

namespace Hyper.NodeServices.Contracts
{
    /// <summary>
    /// The reasons why the <see cref="IHyperNodeService"/> chose to take the <see cref="HyperNodeActionType"/> specified in the <see cref="HyperNodeMessageResponse"/>.
    /// </summary>
    [DataContract]
    public enum HyperNodeActionReasonType
    {
        /// <summary>
        /// Indicates that no reason was specified for the action taken by the receiving <see cref="IHyperNodeService"/>.
        /// </summary>
        [EnumMember]
        Unknown = 0,

        /// <summary>
        /// Indicates that the message was rejected by user-defined code for a user-defined reason.
        /// </summary>
        [EnumMember]
        Custom = 1,

        /// <summary>
        /// Indicates that the message was valid and this <see cref="IHyperNodeService"/> is ready to process it.
        /// </summary>
        [EnumMember]
        ValidMessage = 5,

        /// <summary>
        /// Indicates that the receiving <see cref="IHyperNodeService"/> generated a task ID for a task that was already running.
        /// </summary>
        [EnumMember]
        DuplicateTaskId = 6,

        /// <summary>
        /// Indicates that the receiving <see cref="IHyperNodeService"/> has already reached its maximum number of concurrent tasks.
        /// </summary>
        [EnumMember]
        MaxConcurrentTaskCountReached = 7,

        /// <summary>
        /// Indicates that the Cancel() method has been called on the receiving <see cref="IHyperNodeService"/> and no new tasks are being started.
        /// </summary>
        [EnumMember]
        CancellationRequested = 8,

        /// <summary>
        /// Indicates that the ITaskIdProvider implementation threw an exception while generating a task ID.
        /// </summary>
        [EnumMember]
        TaskIdProviderThrewException = 9,

        /// <summary>
        /// Indicates that the ITaskIdProvider implementation generated an invalid task ID.
        /// </summary>
        [EnumMember]
        InvalidTaskId = 10
    }
}
