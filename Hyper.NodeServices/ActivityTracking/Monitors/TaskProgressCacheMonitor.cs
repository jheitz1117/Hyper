using System;
using System.Runtime.Caching;
using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Extensibility.ActivityTracking;
using Hyper.NodeServices.SystemCommands.Contracts;

namespace Hyper.NodeServices.ActivityTracking
{
    /// <summary>
    /// Collects <see cref="HyperNodeTaskProgressInfo"/> objects based on task ID.
    /// </summary>
    internal sealed class TaskProgressCacheMonitor : HyperNodeServiceActivityMonitor, IDisposable
    {
        private static readonly object Lock = new object();
        private static readonly MemoryCache Cache = MemoryCache.Default;
        private static readonly CacheItemPolicy CachePolicy = new CacheItemPolicy();

        public TimeSpan CacheDuration
        {
            get { return CachePolicy.SlidingExpiration; }
            set { CachePolicy.SlidingExpiration = value; }
        }

        public TaskProgressCacheMonitor()
        {
            this.Name = GetType().Name;
        }

        public override void OnTrack(IHyperNodeActivityEventItem activity)
        {
            // First add a new cache item or get the existing cache item with the specified key
            var taskProgressInfo = AddOrGetExisting(activity.TaskId, () => new HyperNodeTaskProgressInfo());

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
                        ProgressTotal = activity.ProgressTotal,
                        Elapsed = activity.Elapsed
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
                    taskProgressInfo.ChildNodeTaskIds.TryAdd(response.RespondingNodeName, response.TaskId);
                }
            }

            // Make sure our progress properties are updated. If no values were supplied, then we presume there are no updates, but we'll keep any values we had before
            taskProgressInfo.ProgressPart = activity.ProgressPart ?? taskProgressInfo.ProgressPart;
            taskProgressInfo.ProgressTotal = activity.ProgressTotal ?? taskProgressInfo.ProgressTotal;
        }

        public override void OnActivityReportingError(Exception exception)
        {
            // First add a new cache item or get the existing cache item with the specified key
            var taskProgressInfo = AddOrGetExisting("Error", () => new HyperNodeTaskProgressInfo());

            // Now add our specific item to our list of activity items. Need lock because list is not thread-safe
            lock (Lock)
            {
                taskProgressInfo.Activity.Add(
                    new HyperNodeActivityItem("Error")
                    {
                        EventDateTime = DateTime.Now,
                        EventDescription = exception.Message,
                        EventDetail = exception.ToString()
                    }
                );
            }
        }

        /// <summary>
        /// Gets the <see cref="HyperNodeTaskProgressInfo"/> object from the cache with the specified task ID. If no cache item exists with the specified task ID, return null.
        /// </summary>
        /// <param name="taskId">The task ID to look for.</param>
        /// <returns></returns>
        public HyperNodeTaskProgressInfo GetTaskProgressInfo(string taskId)
        {
            return (
                Cache[taskId] as Lazy<HyperNodeTaskProgressInfo> ?? new Lazy<HyperNodeTaskProgressInfo>(
                    () => null // If we don't have any task progress for the specified task ID, just return null
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
