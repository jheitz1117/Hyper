﻿using System.Diagnostics;
using Hyper.NodeServices.Extensibility;

namespace HyperNetExtensibilityTest.ActivityMonitors
{
    /// <summary>
    /// Stock <see cref="HyperNodeServiceActivityMonitor"/> that calls <see cref="Trace"/>.WriteLine() for each activity event. This monitor
    /// may be used during development along with an accompanying set of <see cref="TraceListener"/> objects to output activity to a file or
    /// console application. Alternatively, you can write your own subclass of <see cref="HyperNodeServiceActivityMonitor"/> to better suit
    /// your needs.
    /// </summary>
    public class TaskActivityTracer : HyperNodeServiceActivityMonitor
    {
        public TaskActivityTracer()
        {
            this.Name = GetType().Name;
        }

        public override void OnNext(IHyperNodeActivityEventItem activity)
        {
            Trace.WriteLine(
                string.Format("{0}\r\n{1}\r\n    {2:G} {3}\r\n    {4}\r\n    {5}",
                    activity.MessageGuid,
                    activity.TaskId,
                    activity.EventDateTime,
                    activity.Agent,
                    activity.EventDescription,
                    activity.EventDetail
                )
            );
        }
    }
}
