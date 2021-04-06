using System;
using Hyper.ActivityTracking;
using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.EventTracking;
using Hyper.NodeServices.Extensibility.ActivityTracking;
using Hyper.NodeServices.Extensibility.EventTracking;

namespace Hyper.NodeServices.ActivityTracking.Trackers
{
    internal sealed class HyperNodeTaskActivityTracker : HyperActivityTracker, ITaskActivityTracker
    {
        private readonly ITaskEventContext _taskContext;
        private readonly IHyperNodeEventHandler _eventHandler;
        private readonly Action _cancelTaskAction;

        public HyperNodeTaskActivityTracker(ITaskEventContext taskContext, IHyperNodeEventHandler eventHandler, Action cancelTaskAction)
        {
            _taskContext = taskContext;
            _eventHandler = eventHandler;
            _cancelTaskAction = () =>
            {
                Track("Task cancellation requested from user-defined code.");
                cancelTaskAction();
            };
        }

        public void TrackTaskStarted()
        {
            Track("Task started.");

            try
            {
                _eventHandler.OnTaskStarted(
                    new HyperNodeEventArgs(this, _taskContext)
                );
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
                _eventHandler.OnMessageIgnored(
                    new MessageIgnoredEventArgs(this, _taskContext, reason)
                );
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
                _eventHandler.OnMessageProcessed(
                    new HyperNodeEventArgs(this, _taskContext)
                );
            }
            catch (Exception ex)
            {
                TrackException(ex);
            }
        }

        public void TrackForwardingMessage(string recipient, Action skipRecipientAction)
        {
            Track($"Forwarding message to HyperNode '{recipient}'.");

            try
            {
                _eventHandler.OnForwardingMessage(
                    new ForwardingMessageEventArgs(
                        this,
                        _taskContext,
                        recipient,
                        _cancelTaskAction,
                        skipRecipientAction
                    )
                );
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
                _eventHandler.OnMessageSeen(
                    new MessageSeenEventArgs(this, _taskContext, _cancelTaskAction)
                );
            }
            catch (Exception ex)
            {
                TrackException(ex);
            }
        }

        public void TrackHyperNodeResponded(string childHyperNodeName, HyperNodeMessageResponse response)
        {
            Track(
                $"Response received from HyperNode '{childHyperNodeName}'.",
                string.Join(
                    Environment.NewLine,
                    $"{nameof(response.NodeAction)}:         {response.NodeAction}",
                    $"{nameof(response.NodeActionReason)}:   {response.NodeActionReason}",
                    $"{nameof(response.ProcessStatusFlags)}: {response.ProcessStatusFlags}"
                ),
                response
            );

            try
            {
                _eventHandler.OnHyperNodeResponded(
                    new HyperNodeRespondedEventArgs(
                        this,
                        _taskContext,
                        childHyperNodeName,
                        new ReadOnlyHyperNodeResponseInfo(response)
                    )
                );
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
                _eventHandler.OnTaskCompleted(
                    new TaskCompletedEventArgs(
                        this,
                        _taskContext,
                        new ReadOnlyHyperNodeResponseInfo(response)
                    )
                );
            }
            catch (Exception ex)
            {
                TrackException(ex);
            }
        }

        public void TrackActivityVerbatim(HyperNodeActivityEventItem item)
        {
            OnTrackActivity(
                new TrackActivityEventArgs(item)
            );
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
