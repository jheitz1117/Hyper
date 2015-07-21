using System;
using Hyper.Services.HyperNodeActivityTracking;
using Hyper.Services.HyperNodeContracts;

namespace Hyper.Services.HyperNodeServices
{
    internal sealed class HyperNodeServiceActivityTracker : HyperNodeTaskActivityTracker
    {
        public HyperNodeServiceActivityTracker(string hyperNodeName, HyperNodeMessageRequest messageToTrack)
            : base(hyperNodeName, messageToTrack) { }

        public void TrackCreated()
        {
            Track("Message created.");
        }

        public void TrackReceived()
        {
            Track("Message received.");
        }

        public void TrackIgnored(string reason)
        {
            Track("Message ignored.", reason);
        }

        public void TrackProcessed()
        {
            Track("Message processed.");
        }

        public void TrackForwarding(string recipient)
        {
            TrackFormat("Forwarding message to HyperNode '{0}'.", recipient);
        }

        public void TrackSeen()
        {
            Track("Message seen.");
        }

        public void TrackException(Exception exception)
        {
            Track(exception.Message, exception.ToString());
        }

        public void TrackHyperNodeResponded(string hyperNodeName, HyperNodeMessageResponse response)
        {
            var eventDescription = string.Format("Response received from HyperNode '{0}'.", hyperNodeName);
            var eventDetail = string.Join(
                Environment.NewLine,
                string.Format("NodeAction: {0}", response.NodeAction),
                string.Format("NodeActionReason: {0}", response.NodeActionReason),
                string.Format("ProcessStatusFlags: {0}", response.ProcessStatusFlags)
            );

            Track(eventDescription, eventDetail, response);
        }

        /// <summary>
        /// This method should only ever be called once at the very end of a HyperNode's processing of a message after all of the child threads have completed.
        /// </summary>
        public void TrackFinished()
        {
            TrackFinished("Processing completed.", null, null);
        }
    }
}
