using System;
using System.Collections.Generic;
using System.Runtime.Caching;
using Hyper.Services.HyperNodeContracts;
using Hyper.Services.HyperNodeExtensibility;

namespace Hyper.Services.HyperNodeServices
{
    /// <summary>
    /// This class is responsible for collecting <see cref="HyperNodeActivityItem"/> objects and grouping them by Message GUID.
    /// This provides a way to view a full history of everything that a given HyperNode did with a message, including any
    /// instances where the message returned to the node after having been forwarded, which just results in the node noting
    /// in the history that it received it a second time and is ignoring it
    /// </summary>
    internal sealed class CachedProgressInfoCollector : HyperNodeServiceActivityMonitor, IDisposable
    {
        private static readonly object Lock = new object();
        private static readonly MemoryCache Cache = MemoryCache.Default;
        private static readonly CacheItemPolicy CachePolicy = new CacheItemPolicy();

        public TimeSpan CacheDuration
        {
            get { return CachePolicy.SlidingExpiration; }
            set { CachePolicy.SlidingExpiration = value; }
        }

        public CachedProgressInfoCollector()
        {
            this.Name = GetType().Name;
        }

        public CachedProgressInfoCollector(TimeSpan cacheDuration)
            : this()
        {
            this.CacheDuration = cacheDuration;
        }

        public override void OnNext(IHyperNodeActivityEventItem activity)
        {
            // First add a new list or get the existing list with the specified key
            var progressInfo = AddOrGetExisting(activity.MessageGuid.ToString(), () => new HyperNodeProgressInfo());

            // Now add our specific item to our list of activity items
            lock (Lock)
            {
                progressInfo.Activity.Add(
                    new HyperNodeActivityItem(activity.Agent)
                    {
                        EventDateTime = activity.EventDateTime,
                        EventDescription = activity.EventDescription,
                        EventDetail = activity.EventDetail,
                        ProgressPart = activity.ProgressPart,
                        ProgressTotal = activity.ProgressTotal
                    }
                );

                // Once we get a completion event, we'll stay completed from then on out, even if we get another event later with IsCompletionEvent = false
                progressInfo.IsComplete |= activity.IsCompletionEvent;

                // Make sure our progress properties are updated. If no values were supplied, then we presume there are no updates, but we'll keep any original values we had before
                progressInfo.ProgressPart = activity.ProgressPart ?? progressInfo.ProgressPart;
                progressInfo.ProgressTotal = activity.ProgressTotal ?? progressInfo.ProgressTotal;
            }
        }

        public HyperNodeProgressInfo GetProgressInfo(string key)
        {
            // If we don't have a cache item with the specified key, we'll return a progress info in Completed status that informs the caller that no progress information exists
            // This will prevent the caller from sitting in an infinite loop waiting for IsComplete to be true when there may not be a cache item available, or no cache at all
            return (
                Cache[key] as Lazy<HyperNodeProgressInfo> ?? new Lazy<HyperNodeProgressInfo>(
                    () => new HyperNodeProgressInfo
                    {
                        IsComplete = true,
                        Activity = new List<HyperNodeActivityItem>
                        {
                            new HyperNodeActivityItem
                            {
                                EventDateTime = DateTime.Now,
                                EventDescription = string.Format("No progress information exists for key '{0}'", key),
                                Agent = GetType().Name
                            }
                        }
                    }
                )
            ).Value;
        }

        public void Dispose()
        {
            Dispose(true);
        }

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
    }
}
