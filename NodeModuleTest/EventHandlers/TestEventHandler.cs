using Hyper.NodeServices.Extensibility.EventTracking;

namespace NodeModuleTest.EventHandlers
{
    public class TestEventHandler : HyperNodeEventHandlerBase
    {
        public override void OnMessageReceived(IMessageReceivedEventArgs args)
        {
            if (args.MessageInfo.CommandName == "Echo")
            {
                args.RejectMessage("I hate echos!");
            }
        }

        public override void OnMessageSeen(IMessageSeenEventArgs args)
        {
            if (args.TaskContext.CommandName == "TestLongRunningCommand")
            {
                //args.CancelTask();
            }
        }

        public override void OnTaskStarted(IHyperNodeEventArgs args)
        {
            
        }

        public override void OnForwardingMessage(IForwardingMessageEventArgs args)
        {
            if (args.RecipientNodeName == "Bob")
            {
                //args.Activity.Track("Skipping bob");
                //args.SkipRecipient();
            }
        }
    }
}
