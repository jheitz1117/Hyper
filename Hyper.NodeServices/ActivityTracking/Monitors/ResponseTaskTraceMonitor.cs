using System;
using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Extensibility.ActivityTracking;

namespace Hyper.NodeServices.ActivityTracking
{
    internal sealed class ResponseTaskTraceMonitor : HyperNodeServiceActivityMonitor
    {
        private static readonly object Lock = new object();
        private readonly HyperNodeMessageResponse _target;

        public ResponseTaskTraceMonitor(HyperNodeMessageResponse target)
        {
            Name = nameof(ResponseTaskTraceMonitor);
            _target = target;
        }

        public override void OnTrack(IHyperNodeActivityEventItem activity)
        {
            lock (Lock)
            {
                _target.TaskTrace.Add(
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
        }

        public override void OnActivityReportingError(Exception exception)
        {
            lock (Lock)
            {
                _target.TaskTrace.Add(
                    new HyperNodeActivityItem(Name)
                    {
                        EventDateTime = DateTime.Now,
                        EventDescription = exception.Message,
                        EventDetail = exception.ToString()
                    }
                );
            }
        }
    }
}
