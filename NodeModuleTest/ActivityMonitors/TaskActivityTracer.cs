using System;
using System.Diagnostics;
using Hyper.NodeServices.Extensibility.ActivityTracking;

namespace NodeModuleTest.ActivityMonitors
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
            Name = nameof(TaskActivityTracer);
        }

        public override void OnTrack(IHyperNodeActivityEventItem activity)
        {
            Trace.WriteLine(
                $"{activity.TaskId}\r\n    {activity.EventDateTime:G} {activity.Agent}\r\n    {activity.EventDescription}\r\n    {activity.EventDetail}"
            );
        }

        public override void OnActivityReportingError(Exception exception)
        {
            Trace.WriteLine(exception);
        }

        public override void OnTaskCompleted()
        {
            Trace.WriteLine("Task Completed!");
        }
    }
}
