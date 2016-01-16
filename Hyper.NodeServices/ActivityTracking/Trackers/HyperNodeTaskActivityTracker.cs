using System;
using Hyper.ActivityTracking;
using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Extensibility.ActivityTracking;

namespace Hyper.NodeServices.ActivityTracking
{
    internal sealed class HyperNodeTaskActivityTracker : HyperActivityTracker, ITaskActivityTracker
    {
        private readonly TaskActivityEventContext _context;

        public HyperNodeTaskActivityTracker(TaskActivityEventContext context)
        {
            _context = context;
        }

        public void TrackTaskStarted()
        {
            Track("Task started.");
            _context.EventTracker.TrackTaskStarted(_context.CancelTaskAction);
        }

        public void TrackMessageIgnored(string reason)
        {
            Track("Message ignored.", reason);
            _context.EventTracker.TrackMessageIgnored(reason);
        }

        public void TrackMessageProcessed()
        {
            Track("Message processed.");
            _context.EventTracker.TrackMessageProcessed();
        }

        public void TrackForwardingMessage(string recipient)
        {
            TrackFormat("Forwarding message to HyperNode '{0}'.", recipient);
            _context.EventTracker.TrackForwardingMessage(recipient);
        }

        public void TrackMessageSeen()
        {
            Track("Message seen.");
            _context.EventTracker.TrackMessageSeen(_context.CancelTaskAction);
        }

        public void TrackHyperNodeResponded(string childHyperNodeName, HyperNodeMessageResponse response)
        {
            var eventDescription = string.Format("Response received from HyperNode '{0}'.", childHyperNodeName);
            var eventDetail = string.Join(
                Environment.NewLine,
                string.Format("NodeAction: {0}", response.NodeAction),
                string.Format("NodeActionReason: {0}", response.NodeActionReason),
                string.Format("ProcessStatusFlags: {0}", response.ProcessStatusFlags)
            );

            Track(eventDescription, eventDetail, response);
            _context.EventTracker.TrackHyperNodeResponded(childHyperNodeName, response);
        }

        /// <summary>
        /// This method should only ever be called once at the very end of a HyperNode's processing of a message after all of the child threads have completed.
        /// </summary>
        /// <param name="response">The complete <see cref="HyperNodeMessageResponse"/> object to report.</param>
        public void TrackTaskComplete(HyperNodeMessageResponse response)
        {
            Track("Task complete.", null, response);
            _context.EventTracker.TrackTaskComplete(response);
        }

        #region ITaskActivityTracker Implementation

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
            OnTrackActivity(
                new TrackActivityEventArgs(
                    new HyperNodeActivityEventItem
                    {
                        Agent = _context.HyperNodeName,
                        TaskId = _context.TaskId,
                        CommandName = _context.CommandName,
                        Elapsed = _context.Elapsed,
                        EventDateTime = DateTime.Now,
                        EventDescription = eventDescription,
                        EventDetail = eventDetail,
                        EventData = eventData,
                        ProgressPart = progressPart,
                        ProgressTotal = progressTotal
                    }
                )
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

        #endregion ITaskActivityTracker Implementation
    }
}
