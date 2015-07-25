using System;
using Hyper.ActivityTracking;
using Hyper.Services.HyperNodeContracts;
using Hyper.Services.HyperNodeExtensibility;

namespace Hyper.Services.HyperNodeServices
{
    internal class HyperNodeTaskActivityTracker : HyperActivityTracker, ITaskActivityTracker
    {
        private readonly string _hyperNodeName;
        private readonly Guid _messageGuid;
        private readonly string _commandName;

        public HyperNodeTaskActivityTracker(string hyperNodeName, HyperNodeMessageRequest messageToTrack)
        {
            _hyperNodeName = hyperNodeName;
            _messageGuid = messageToTrack.MessageGuid;
            _commandName = messageToTrack.CommandName;
        }

        public void Track(string eventDescription)
        {
            Track(eventDescription, null);
        }

        public void TrackFormat(string eventDescription, params object[] args)
        {
            Track(string.Format(eventDescription, args));
        }

        public void Track(string eventDescription, string eventDetail)
        {
            Track(eventDescription, eventDetail, null);
        }

        public void Track(string eventDescription, string eventDetail, object eventData)
        {
            Track(eventDescription, eventDetail, eventData, null, null, false);
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
            Track(eventDescription, eventDetail, eventData, progressPart, progressTotal, false);
        }

        /// <summary>
        /// Raises an event with IsCompleted set to true. This method is internal to prevent user code from calling it.
        /// </summary>
        /// <param name="eventDescription"></param>
        /// <param name="eventDetail"></param>
        /// <param name="eventData"></param>
        internal protected void TrackFinished(string eventDescription, string eventDetail, object eventData)
        {
            Track(eventDescription, eventDetail, eventData, null, null, true);
        }

        private void Track(string eventDescription, string eventDetail, object eventData, double? progressPart, double? progressTotal, bool isCompletionEvent)
        {
            OnTrackActivity(
                new TrackActivityEventArgs(
                    new HyperNodeActivityEventItem(isCompletionEvent)
                    {
                        MessageGuid = _messageGuid,
                        CommandName = _commandName,
                        EventDateTime = DateTime.Now,
                        Agent = _hyperNodeName,
                        EventDescription = eventDescription,
                        EventDetail = eventDetail,
                        EventData = eventData,
                        ProgressPart = progressPart,
                        ProgressTotal = progressTotal
                    }
                )
            );
        }
    }
}
