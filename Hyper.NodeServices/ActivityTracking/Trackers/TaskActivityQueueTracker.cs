using System;
using System.Collections.Generic;
using Hyper.NodeServices.Extensibility.ActivityTracking;
using Hyper.NodeServices.Extensibility.EventTracking;

namespace Hyper.NodeServices.ActivityTracking.Trackers
{
    internal class TaskActivityQueueTracker : Queue<HyperNodeActivityEventItem>, ITaskActivityTracker
    {
        private readonly ITaskEventContext _taskContext;

        public TaskActivityQueueTracker(ITaskEventContext taskContext)
        {
            _taskContext = taskContext;
        }

        public void Track(string eventDescription)
        {
            Track(eventDescription, null);
        }

        public void Track(string eventDescription, string eventDetail)
        {
            Track(eventDescription, eventDetail, null);
        }

        public void Track(string eventDescription, string eventDetail, object eventData)
        {
            Track(eventDescription, eventDetail, eventData, null, null);
        }

        public void Track(string eventDescription, double? progressPart, double? progressTotal)
        {
            Track(eventDescription, null, progressPart, progressTotal);
        }

        public void Track(string eventDescription, string eventDetail, double? progressPart, double? progressTotal)
        {
            Track(eventDescription, eventDetail, null, progressPart, progressTotal);
        }

        public void Track(string eventDescription, string eventDetail, object eventData, double? progressPart, double? progressTotal)
        {
            Enqueue(
                new HyperNodeActivityEventItem
                {
                    Agent = _taskContext.HyperNodeName,
                    TaskId = _taskContext.TaskId,
                    CommandName = _taskContext.CommandName,
                    Elapsed = _taskContext.Elapsed,
                    EventDateTime = DateTime.Now,
                    EventDescription = eventDescription,
                    EventDetail = eventDetail,
                    EventData = eventData,
                    ProgressPart = progressPart,
                    ProgressTotal = progressTotal
                }
            );
        }

        public void TrackFormat(string eventDescriptionFormat, params object[] args)
        {
            Track(string.Format(eventDescriptionFormat, args));
        }

        public void TrackException(Exception exception)
        {
            Track(exception.Message, exception.ToString());
        }
    }
}
