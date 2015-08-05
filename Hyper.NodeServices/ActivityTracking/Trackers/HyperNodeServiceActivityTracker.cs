using System;
using Hyper.NodeServices.Contracts;

namespace Hyper.NodeServices.ActivityTracking
{
    internal sealed class HyperNodeServiceActivityTracker : HyperNodeTaskActivityTracker
    {
        public HyperNodeServiceActivityTracker(HyperNodeActivityContext context)
            : base(context) { }

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
        /// <param name="response">The complete <see cref="HyperNodeMessageResponse"/> object to report.</param>
        public void TrackFinished(HyperNodeMessageResponse response)
        {
            TrackFinished("Processing completed.", null, response);
        }
    }
}
