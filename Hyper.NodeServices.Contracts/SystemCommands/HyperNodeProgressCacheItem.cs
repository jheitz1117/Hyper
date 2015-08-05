using System.Collections.Concurrent;
using System.Runtime.Serialization;

namespace Hyper.NodeServices.Contracts.SystemCommands
{
    [DataContract]
    public class HyperNodeProgressCacheItem
    {
        [DataMember]
        public ConcurrentDictionary<string, HyperNodeTaskProgressInfo> TaskProgress { get; set; }

        public HyperNodeProgressCacheItem()
        {
            this.TaskProgress = new ConcurrentDictionary<string, HyperNodeTaskProgressInfo>();
        }
    }
}
