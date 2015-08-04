using System.Threading;
using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Contracts.Extensibility;
using Hyper.NodeServices.Extensibility;

namespace Hyper.NodeServices.CommandModules.TestCommands
{
    internal class LongRunningCommandTest : ICommandModule
    {
        public ICommandResponse Execute(ICommandExecutionContext context)
        {
            var progressTotal = 100;
            context.Activity.TrackFormat("CommandName '{0}' recognized. Commencing execution of task.", context.CommandName);
            Thread.Sleep(3000);
            context.Activity.Track("Progress update 1", 10, progressTotal);
            Thread.Sleep(3000);
            context.Activity.Track("Progress update 2", 20, progressTotal);
            Thread.Sleep(3000);
            context.Activity.Track("Progress update 3", 30, progressTotal);
            Thread.Sleep(3000);
            context.Activity.Track("Progress update 4", 44, progressTotal);
            Thread.Sleep(3000);
            context.Activity.Track("Progress update 5", 53, progressTotal);
            Thread.Sleep(3000);
            context.Activity.Track("Progress update 6", 72, progressTotal);
            Thread.Sleep(6000);
            context.Activity.Track("Progress update 7", 95, progressTotal);

            context.Activity.Track("Progress update 8", progressTotal, progressTotal);

            return new CommandResponse(MessageProcessStatusFlags.Success);
        }
    }
}
