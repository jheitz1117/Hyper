using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Extensibility;

namespace Hyper.NodeServices.ActivityTracking
{
    internal sealed class HyperNodeTaskTraceCollector : HyperNodeServiceActivityMonitor
    {
        private static readonly object Lock = new object();
        private readonly HyperNodeMessageResponse _target;

        public HyperNodeTaskTraceCollector(HyperNodeMessageResponse target)
        {
            this.Name = GetType().Name;
            _target = target;
        }

        public override void OnNext(IHyperNodeActivityEventItem activity)
        {
            lock (Lock)
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
