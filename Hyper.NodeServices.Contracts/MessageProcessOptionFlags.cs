using System;
using System.Runtime.Serialization;

namespace Hyper.NodeServices.Contracts
{
    /********************************************************************************
     * Using bit-shifts such as 1 << 2 are fast and super-easy to understand.
     * They do powers of 2 for you so you don't have to keep track of them yourself.
     ********************************************************************************/

    /// <summary>
    /// Flags that indicate what happened while a command was running in a <see cref="IHyperNodeService"/>
    /// </summary>
    [Flags]
    [DataContract]
    public enum MessageProcessOptionFlags
    {
        /// <summary>
        /// Specifies that the recipient node should process this message synchronously without returning a task trace or caching activity events
        /// </summary>
        [EnumMember]
        None = 0,

        /// <summary>
        /// Specifies that the recipient node should return an activity event trace for this message.
        /// </summary>
        [EnumMember]
        ReturnTaskTrace = 1 << 0,

        /// <summary>
        /// Specifies that the recipient node should run the request in a child thread.
        /// </summary>
        [EnumMember]
        RunConcurrently = 1 << 1,

        /// <summary>
        /// Specifies that the recipient node should cache task progress for this message.
        /// </summary>
        [EnumMember]
        CacheTaskProgress = 1 << 2,
    }
}
