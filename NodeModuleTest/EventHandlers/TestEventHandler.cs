using System.Threading;
using Hyper.NodeServices.Extensibility.EventTracking;

namespace NodeModuleTest.EventHandlers
{
    public class TestEventHandler : HyperNodeEventHandlerBase
    {
        public override void OnMessageReceived(IMessageReceivedEventArgs args)
        {
            args.Activity.Track("This is 1 from OnMessageReceived!", "WOW!!!");
            Thread.Sleep(100);
            args.Activity.Track("This is 2 from OnMessageReceived!", "WOW!!!");
            Thread.Sleep(100);
            args.Activity.Track("This is 3 from OnMessageReceived!", "WOW!!!");
            Thread.Sleep(100);
            args.Activity.Track("This is 4 from OnMessageReceived!", "WOW!!!");
            Thread.Sleep(100);
            args.Activity.Track("This is 5 from OnMessageReceived!", "WOW!!!");
            Thread.Sleep(100);

            if (args.TaskContext.CommandName == "TestLongRunningCommand2")
            {
                args.RejectMessage("Barnacle!");
            }
        }

        public override void OnMessageSeen(IMessageSeenEventArgs args)
        {
            if (args.TaskContext.CommandName == "TestLongRunningCommand")
            {
                args.CancelTask();
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
