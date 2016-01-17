using System;
using Hyper.ActivityTracking;
using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.EventTracking;
using Hyper.NodeServices.Extensibility.ActivityTracking;
using Hyper.NodeServices.Extensibility.EventTracking;

namespace Hyper.NodeServices.ActivityTracking
{
    internal sealed class HyperNodeTaskActivityTracker : HyperActivityTracker, ITaskActivityTracker
    {
        private readonly ITaskEventContext _taskContext;
        private readonly HyperNodeEventTracker _eventTracker;

        public HyperNodeTaskActivityTracker(ITaskEventContext taskContext, IHyperNodeEventHandler eventHandler, Action<string> rejectMessageAction, Action cancelTaskAction)
        {
            _taskContext = taskContext;
            _eventTracker = new HyperNodeEventTracker(this, taskContext, eventHandler, rejectMessageAction, cancelTaskAction);
        }

        public void TrackTaskStarted()
        {
            Track("Task started.");

            try
            {
                _eventTracker.TrackTaskStarted();
            }
            catch (Exception ex)
            {
                TrackException(ex);
            }
        }

        public void TrackMessageIgnored(string reason)
        {
            Track("Message ignored.", reason);

            try
            {
                _eventTracker.TrackMessageIgnored(reason);
            }
            catch (Exception ex)
            {
                TrackException(ex);
            }
        }

        public void TrackMessageProcessed()
        {
            Track("Message processed.");

            try
            {
                _eventTracker.TrackMessageProcessed();
            }
            catch (Exception ex)
            {
                TrackException(ex);
            }
        }

        public void TrackForwardingMessage(string recipient)
        {
            TrackFormat("Forwarding message to HyperNode '{0}'.", recipient);

            try
            {
                _eventTracker.TrackForwardingMessage(recipient);
            }
            catch (Exception ex)
            {
                TrackException(ex);
            }
        }

        public void TrackMessageSeen()
        {
            Track("Message seen.");

            try
            {
                _eventTracker.TrackMessageSeen();
            }
            catch (Exception ex)
            {
                TrackException(ex);
            }
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

            try
            {
                _eventTracker.TrackHyperNodeResponded(childHyperNodeName, response);
            }
            catch (Exception ex)
            {
                TrackException(ex);
            }
        }

        /// <summary>
        /// This method should only ever be called once at the very end of a HyperNode's processing of a message after all of the child threads have completed.
        /// </summary>
        /// <param name="response">The complete <see cref="HyperNodeMessageResponse"/> object to report.</param>
        public void TrackTaskComplete(HyperNodeMessageResponse response)
        {
            Track("Task complete.", null, response);

            try
            {
                _eventTracker.TrackTaskComplete(response);
            }
            catch (Exception ex)
            {
                TrackException(ex);
            }
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
