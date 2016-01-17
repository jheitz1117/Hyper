using System;
using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Extensibility;
using Hyper.NodeServices.Extensibility.ActivityTracking;
using Hyper.NodeServices.Extensibility.EventTracking;

namespace Hyper.NodeServices.EventTracking
{
    internal sealed class HyperNodeEventTracker
    {
        private readonly IHyperNodeEventHandler _eventHandler;
        private readonly ITaskEventContext _taskContext;
        private readonly ITaskActivityTracker _activityTracker;

        private readonly Action<string> _rejectMessageAction;
        private readonly Action _cancelTaskAction;

        public HyperNodeEventTracker(ITaskActivityTracker activityTracker, ITaskEventContext taskContext, IHyperNodeEventHandler eventHandler, Action<string> rejectMessageAction, Action cancelTaskAction)
        {
            _eventHandler = eventHandler;
            _taskContext = taskContext;
            _activityTracker = activityTracker;
            _rejectMessageAction = rejectMessageAction;
            _cancelTaskAction = cancelTaskAction;
        }

        // TODO: Finish implementing these events

        public void TrackMessageReceived(IHyperNodeMessageContext messageContext)
        {
            // TODO: This event is special because it's the only one that doesn't have a ITaskEventContext, because no task ID has been created yet. It's also the only one called directly from the hypernode instead of from the activity tracker.
            // TODO: Should this event really be treated special?
            _eventHandler.OnMessageReceived(
                new MessageReceivedEventArgs(
                    _taskContext.HyperNodeName,
                    messageContext,
                    _rejectMessageAction
                )
            );
        }

        public void TrackTaskStarted()
        {
            _eventHandler.OnTaskStarted(
                new TaskStartedEventArgs(_activityTracker, _taskContext, _cancelTaskAction)
            );
        }

        public void TrackMessageIgnored(string reason)
        {
            //_eventHandler.OnMessageIgnored(
            //    _taskContext,
            //    new MessageIgnoredEventArgs(
            //        reason
            //    )
            //);
        }

        public void TrackMessageProcessed()
        {
            //_eventHandler.OnMessageProcessed(
            //    _taskContext
            //    new MessageProcessedEventArgs(
            //    )
            //);
        }

        public void TrackForwardingMessage(string recipient)
        {
            //_eventHandler.OnBeforeMessageForwarded(
            //    _taskContext,
            //    new BeforeMessageForwardedEventArgs(
            //        recipient,
            //        cancelTaskAction,
            //        cancelForwardingAction
            //    )
            //);
        }

        public void TrackMessageSeen()
        {
            //_eventHandler.OnMessageSeen(
            //    _taskContext,
            //    new MessageSeenEventArgs(
            //        _cancelTaskAction
            //    )
            //);
        }

        public void TrackHyperNodeResponded(string childHyperNodeName, HyperNodeMessageResponse response)
        {
            //_eventHandler.OnHyperNodeResponded(
            //    _taskContext,
            //    new HyperNodeRespondedEventArgs(
            //        childHyperNodeName,
            //        childResponse
            //    )
            //);
        }

        public void TrackTaskComplete(HyperNodeMessageResponse response)
        {
            //_eventHandler.OnTaskCompleted(
            //    _taskContext,
            //    new TaskCompletedEventArgs(
            //        response
            //    )
            //);
        }
    }

    // Example of inherting HyperNodeEventHandlerBase...
    public class UserDefinedHyperNodeEventHandler : HyperNodeEventHandlerBase
    {
        public override void OnMessageReceived(IMessageReceivedEventArgs args)
        {

            // TODO: Example of how a user might choose to use this event handler
            if (args.MessageContext.CommandName == "SpeedyCommand")
            {
                args.RejectMessage("Node '" + args.HyperNodeName + "' took too long to process.");
            }
        }

        public override void OnTaskStarted(ITaskStartedEventArgs args)
        {
            // TODO: Example of how a user might choose to use this event handler

            
            args.Activity.Track("Checking elapsed time.");
            if (args.TaskContext.Elapsed.HasValue && args.TaskContext.Elapsed > TimeSpan.FromSeconds(30))
            {
                if (args.TaskContext.CommandName == "Oh No!")
                {
                    args.CancelTask();
                }
            }
            args.Activity.Track("Time check complete.");
        }
    }
}
