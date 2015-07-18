using System;
using System.Diagnostics;
using Hyper.Services.HyperNodeServices;

namespace Hyper.Services.HyperNodeHosting
{
    /// <summary>
    /// Stock <see cref="HyperNodeServiceActivityMonitor"/> that calls <see cref="Trace"/>.WriteLine() for each activity event. This monitor
    /// may be used during development along with an accompanying set of <see cref="TraceListener"/> objects to output activity to a file or
    /// console application. Alternatively, you can write your own subclass of <see cref="HyperNodeServiceActivityMonitor"/> to better suit
    /// your needs.
    /// </summary>
    public sealed class TaskActivityTracer : HyperNodeServiceActivityMonitor
    {
        public TaskActivityTracer()
        {
            this.Name = GetType().Name;
        }

        public override void OnNext(HyperNodeActivityEventItem activity)
        {
            Trace.WriteLine(
                string.Format("{0}  {1:G} {2} - {3}\r\n{4}",
                    activity.MessageGuid,
                    activity.EventDateTime,
                    activity.Agent,
                    activity.EventDescription,
                    activity.EventDetail
                )
            );
        }

        public override void OnCompleted()
        {
            Trace.WriteLine("Event stream has signaled completion.");
        }

        public override void OnError(Exception error)
        {
            Trace.WriteLine("An exception was thrown while tracing tracking activity. Exception information follows:\r\n" + error);
        }
    }
}
