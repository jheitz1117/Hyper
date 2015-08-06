﻿using System;
using Hyper.ActivityTracking;
using Hyper.NodeServices.Extensibility;

namespace Hyper.NodeServices.ActivityTracking
{
    internal class HyperNodeTaskActivityTracker : HyperActivityTracker, ITaskActivityTracker
    {
        private readonly HyperNodeActivityContext _context;
        
        public HyperNodeTaskActivityTracker(HyperNodeActivityContext context)
        {
            _context = context;
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
            OnTrackActivity(
                new TrackActivityEventArgs(
                    new HyperNodeActivityEventItem
                    {
                        MessageGuid = _context.MessageGuid,
                        TaskId = _context.TaskId,
                        CommandName = _context.CommandName,
                        EventDateTime = DateTime.Now,
                        Agent = _context.HyperNodeName,
                        EventDescription = eventDescription,
                        EventDetail = eventDetail,
                        EventData = eventData,
                        ProgressPart = progressPart,
                        ProgressTotal = progressTotal
                    }
                )
            );
        }

        public void TrackFormat(string eventDescription, params object[] args)
        {
            Track(string.Format(eventDescription, args));
        }

        public void TrackException(Exception exception)
        {
            Track(exception.Message, exception.ToString());
        }
    }
}
