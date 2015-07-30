using System;
using System.Runtime.Serialization;

namespace Hyper.NodeServices.Contracts
{
    /********************************************************************************
     * Using bit-shifts such as 1 << 2 are fast and super-easy to understand.
     * They do powers of 2 for you so you don't have to keep track of them yourself.
     ********************************************************************************/

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
