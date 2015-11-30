using System;
using System.Collections.Generic;
using System.Diagnostics;
using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Extensibility;
using Hyper.NodeServices.Extensibility.Configuration.Validation;

namespace Hyper.NodeServices.EventTracking
{
    // This is the class used by the hypernode to fire events
    internal sealed class HyperNodeEventTracker
    {
        private readonly HyperNodeEventHandler _eventHandler;
        private readonly HyperNodeEventContext _context;

        public HyperNodeEventTracker(HyperNodeEventContext context, HyperNodeEventHandler eventHandler)
        {
            _context = context;
            _eventHandler = eventHandler;
        }

        public void TrackMessageReceived(IHyperNodeMessageContext messageContext, Action<string> rejectMessageAction)
        {
            try
            {
                _eventHandler.OnMessageReceived(
                    new MessageReceivedEventArgs(
                        _context, // TODO: This exposes an Elapsed property that is misleading. It is the elapsed time since the node started, which could be quite a while ago and may be null if diagnostics are turned off. Need to make this more clear.
                        messageContext,
                        rejectMessageAction
                    )
                );
            }
            catch (Exception ex)
            {
                // TODO: Figure out what to do in this situation...
                throw;
            }
        }

        public void TrackTaskStarted(Action cancelTaskAction)
        {
            try
            {
                _eventHandler.OnTaskStarted(
                    new TaskStartedEventArgs(
                        _context,
                        cancelTaskAction
                    )
                );
            }
            catch (Exception ex)
            {
                // TODO: Figure out what to do in this situation...
                throw;
            }
        }

        public void TrackMessageIgnored(string reason)
        {
            try
            {
                _eventHandler.OnMessageIgnored(
                    new MessageIgnoredEventArgs(
                        _context,
                        reason
                    )
                );
            }
            catch (Exception ex)
            {
                // TODO: Figure out what to do in this situation...
                throw;
            }
        }

        public void TrackMessageProcessed()
        {
            try
            {
                _eventHandler.OnMessageProcessed(
                    new MessageProcessedEventArgs(
                        _context
                    )
                );
            }
            catch (Exception ex)
            {
                // TODO: Figure out what to do in this situation...
                throw;
            }
        }

        public void TrackForwardingMessage(string recipient)
        {
            try
            {
                _eventHandler.OnBeforeMessageForwarded(
                    new BeforeMessageForwardedEventArgs(
                        _context,
                        recipient,
                        cancelTaskAction,
                        cancelForwardingAction
                    )
                );
            }
            catch (Exception ex)
            {
                // TODO: Figure out what to do in this situation...
                throw;
            }
        }

        public void TrackMessageSeen(Action cancelTaskAction)
        {
            try
            {
                _eventHandler.OnMessageSeen(
                    new MessageSeenEventArgs(
                        _context,
                        cancelTaskAction
                    )
                );
            }
            catch (Exception ex)
            {
                // TODO: Figure out what to do in this situation...
                throw;
            }
        }

        public void TrackHyperNodeResponded(string childHyperNodeName, HyperNodeMessageResponse response)
        {
            try
            {
                _eventHandler.OnHyperNodeResponded(
                    new HyperNodeRespondedEventArgs(
                        _context,
                        childHyperNodeName,
                        childResponse
                    )
                );
            }
            catch (Exception ex)
            {
                // TODO: Figure out what to do in this situation...
                throw;
            }
        }

        public void TrackTaskComplete(HyperNodeMessageResponse response)
        {
            try
            {
                _eventHandler.OnTaskCompleted(
                    new TaskCompletedEventArgs(
                        _context,
                        response
                    )
                );
            }
            catch (Exception ex)
            {
                // TODO: Figure out what to do in this situation...
                throw;
            }
        }
    }

    // This is the class that contains the ability to perform diagnostic information. Needs to have a public-facing interface instead of being public
    public class HyperNodeEventContext
    {
        private readonly string _hyperNodeName;
        private readonly Stopwatch _stopwatch;

        public string HyperNodeName { get { return _hyperNodeName; } }

        public TimeSpan? Elapsed
        {
            get
            {
                TimeSpan? elapsed = null;
                
                if (_stopwatch != null)
                {
                    elapsed = _stopwatch.Elapsed;

                    // The reason we don't start the stopwatch *before* reporting the elapsed time is that we always want the very first
                    // call to Elapsed to return 00:00:00
                    if (!_stopwatch.IsRunning)
                        _stopwatch.Start();
                }

                return elapsed;
            }
        }

        public HyperNodeEventContext(string hyperNodeName, bool enableDiagnostics)
        {
            _hyperNodeName = hyperNodeName;

            if (enableDiagnostics)
                _stopwatch = new Stopwatch();
        }
    }

    // This is the class users will inherit to process certain pieces of the events
    public abstract class HyperNodeEventHandler
    {
        // TODO: The args will allow the user to affect the behavior of the hypernode
        public virtual void OnMessageReceived(MessageReceivedEventArgs args) { }

        // TODO: Create other events here...?
        public virtual void OnOtherEvent(HyperNodeEventContext context) { }
    }

    // Example of inherting HyperNodeEventHandler...
    public class UserDefinedHyperNodeEventHandler : HyperNodeEventHandler
    {
        public override void OnMessageReceived(MessageReceivedEventArgs args)
        {
            // TODO: Example of how a user might choose to use this event handler
            // TODO: Note the availability of both the HyperNode context AND the message context (stolen from ITaskIdProvider)
            if (args.NodeContext.Elapsed.HasValue && args.NodeContext.Elapsed > TimeSpan.FromSeconds(30))
            {
                if (args.MessageContext.CommandName == "SpeedyCommand")
                {
                    args.RejectMessage("Took too long to process.");
                }
            }
        }
    }

    // These are created by the HyperNodeEventTracker and passed to each event. This class needs to set some properties somewhere that can be checked in the HyperNodeService so that the user can affect the behavior of the service.
    public sealed class MessageReceivedEventArgs : EventArgs
    {
        private readonly Action<string> _rejectMessageAction;

        public HyperNodeEventContext NodeContext
        {
            get { return _nodeContext; }
        } private readonly HyperNodeEventContext _nodeContext;

        public IHyperNodeMessageContext MessageContext
        {
            get { return _messageContext; }
        } private readonly IHyperNodeMessageContext _messageContext;

        public MessageReceivedEventArgs(HyperNodeEventContext nodeContext, IHyperNodeMessageContext messageContext, Action<string> rejectMessageAction)
        {
            _nodeContext = nodeContext;
            _messageContext = messageContext;
            _rejectMessageAction = rejectMessageAction;
        }

        public void RejectMessage(string reason)
        {
            if (_rejectMessageAction != null)
                _rejectMessageAction(reason);
        }
    }
}
