using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Contracts.SystemCommands;
using Hyper.NodeServices.Extensibility.ActivityTracking;

namespace Hyper.NodeServices.ActivityTracking
{
    /// <summary>
    /// This class is responsible for collecting <see cref="HyperNodeProgressCacheItem"/> objects and grouping them by Message GUID.
    /// This provides a way to view a full history of everything that a given HyperNode did with a message, including any
    /// instances where the message returned to the node after having been forwarded, which just results in the node noting
    /// in the history that it received it a second time and is ignoring it
    /// </summary>
    internal sealed class ProgressCacheItemCollector : HyperNodeServiceActivityMonitor, IDisposable
    {
        private static readonly object Lock = new object();
        private static readonly MemoryCache Cache = MemoryCache.Default;
        private static readonly CacheItemPolicy CachePolicy = new CacheItemPolicy();

        public TimeSpan CacheDuration
        {
            get { return CachePolicy.SlidingExpiration; }
            set { CachePolicy.SlidingExpiration = value; }
        }

        public ProgressCacheItemCollector()
        {
            this.Name = GetType().Name;
        }

        public override void OnNext(IHyperNodeActivityEventItem activity)
        {
            // First add a new cache item or get the existing cache item with the specified key
            var cacheItem = AddOrGetExisting(activity.MessageGuid.ToString(), () => new HyperNodeProgressCacheItem());

            // Now get or add a task progress info object for this task ID
            var taskProgressInfo = cacheItem.TaskProgress.GetOrAdd(activity.TaskId, s => new HyperNodeTaskProgressInfo(activity.MessageGuid));

            // Now add our specific item to our list of activity items. Need lock because list is not thread-safe
            lock (Lock)
            {
                taskProgressInfo.Activity.Add(
                    new HyperNodeActivityItem(activity.Agent)
                    {
                        EventDateTime = activity.EventDateTime,
                        EventDescription = activity.EventDescription,
                        EventDetail = activity.EventDetail,
                        ProgressPart = activity.ProgressPart,
                        ProgressTotal = activity.ProgressTotal
                    }
                );
            }

            // Check if we were given a response to cache
            var response = activity.EventData as HyperNodeMessageResponse;
            if (response != null)
            {
                /* If the response belongs to this node, then this is a completion event and we're done. We just set our response property to the completed response object we received.
                 * We should only ever get one response object from this node.*/
                if (response.RespondingNodeName == activity.Agent)
                {
                    taskProgressInfo.IsComplete = true;
                    taskProgressInfo.Response = response;
                }
                else
                {
                    /* Otherwise, we assume the response came from a child node. In this case, we'll add the task ID from
                     * the child node's response object to our collection so we can propagate to the caller if necessary.*/
                    taskProgressInfo.ChildTaskIds.TryAdd(response.RespondingNodeName, response.TaskId);
                }
            }

            // Make sure our progress properties are updated. If no values were supplied, then we presume there are no updates, but we'll keep any values we had before
            taskProgressInfo.ProgressPart = activity.ProgressPart ?? taskProgressInfo.ProgressPart;
            taskProgressInfo.ProgressTotal = activity.ProgressTotal ?? taskProgressInfo.ProgressTotal;
        }

        public HyperNodeTaskProgressInfo GetTaskProgressInfo(Guid messageGuid, string taskId)
        {
            // If we can't find any task progress info for the specified Task ID, we'll return a placeholder object in Completed status that informs the caller that no progress
            // information exists for this task ID. This will prevent the caller from sitting in an infinite loop waiting for IsComplete to be true when there may not be a cache
            // item available, or no cache at all
            HyperNodeTaskProgressInfo taskProgressInfo;
            if (!GetProgressCacheItem(messageGuid).TaskProgress.TryGetValue(taskId, out taskProgressInfo))
            {
                taskProgressInfo = new HyperNodeTaskProgressInfo
                {
                    IsComplete = true,
                    Activity = new List<HyperNodeActivityItem>
                    {
                        new HyperNodeActivityItem
                        {
                            EventDateTime = DateTime.Now,
                            EventDescription = string.Format("No task progress information exists for task ID '{0}'", taskId),
                            Agent = GetType().Name
                        }
                    }
                };
            }
            
            return taskProgressInfo;
        }

        /// <summary>
        /// Gets the <see cref="HyperNodeProgressCacheItem"/> object from the cache with the specified message GUID. If no such item exists, return a new, empty cache item.
        /// </summary>
        /// <param name="messageGuid">The <see cref="Guid"/> of the message to look for.</param>
        /// <returns></returns>
        public HyperNodeProgressCacheItem GetProgressCacheItem(Guid messageGuid)
        {
            return (
                Cache[messageGuid.ToString()] as Lazy<HyperNodeProgressCacheItem> ?? new Lazy<HyperNodeProgressCacheItem>(
                    () => new HyperNodeProgressCacheItem()
                )
            ).Value;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #region Private Methods

        /// <summary>
        /// This method wraps the <see cref="MemoryCache"/> object's AddOrGetExisting() method. The need for the wrapper is obscure and only becomes evident upon close examination.
        /// It turns out that <see cref="MemoryCache"/>'s AddOrGetExisting() method does not return the new value you're giving it, but rather the old value that was there before.
        /// For new inserts, this old value is considered to be null, so you get back a null value instead of the value you tried to insert as you might have expected. This wrapper
        /// accounts for that small caveat by utilizing .NET's <see cref="Lazy&lt;T&gt;"/> class.
        /// 
        /// This wrapper method was taken from Adam Anderson's blog at http://blog.falafel.com/working-system-runtime-caching-memorycache/.
        /// </summary>
        /// <typeparam name="T">Type of object being stored in the <see cref="MemoryCache"/></typeparam>
        /// <param name="key">A unique identifier for the cache entry to add or get.</param>
        /// <param name="valueFactory">The delegate that is invoked to produce the lazily initialized value when it is needed.</param>
        /// <returns></returns>
        private static T AddOrGetExisting<T>(string key, Func<T> valueFactory)
        {
            var newValue = new Lazy<T>(valueFactory);
            var oldValue = Cache.AddOrGetExisting(key, newValue, CachePolicy) as Lazy<T>;
            try
            {
                return (oldValue ?? newValue).Value;
            }
            catch
            {
                // Handle cached lazy exception by evicting from cache
                Cache.Remove(key);
                throw;
            }
        }

        private static void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (Cache != null)
                    Cache.Dispose();
            }
        }

        #endregion Private Methods
    }
}
