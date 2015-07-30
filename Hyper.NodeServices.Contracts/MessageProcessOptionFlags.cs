using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Hyper.NodeServices.Contracts
{
    /********************************************************************************
     * Using bit-shifts such as 1 << 2 are fast and super-easy to understand.
     * They do powers of 2 for you so you don't have to keep track of them yourself.
     ********************************************************************************/

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
        /// Specifies that the recipient node should cache activity events for this message.
        /// </summary>
        [EnumMember]
        CacheProgressInfo = 1 << 2,
    }
}
