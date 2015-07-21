using Hyper.Services.HyperNodeActivityTracking;
using Hyper.Services.HyperNodeContracts;
using Hyper.Services.HyperNodeExtensibility;

namespace Hyper.Services.HyperNodeServices
{
    internal sealed class HyperNodeTaskTraceCollector : HyperNodeServiceActivityMonitor
    {
        private readonly object _lock = new object();
        private readonly HyperNodeMessageResponse _target;

        public HyperNodeTaskTraceCollector(HyperNodeMessageResponse target)
        {
            this.Name = GetType().Name;
            _target = target;
        }

        public override void OnNext(HyperNodeActivityEventItem activity)
        {
            lock (_lock)
            {
                _target.TaskTrace.Add(
                    new HyperNodeActivityItem(activity.Agent)
                    {
                        EventDateTime = activity.EventDateTime,
                        EventDescription = activity.EventDescription,
                        EventDetail = activity.EventDetail
                    }
                );
            }
        }
    }
}
