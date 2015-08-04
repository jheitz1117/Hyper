using System;
using System.Diagnostics;
using System.Threading;
using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Contracts.Extensibility;
using Hyper.NodeServices.Extensibility;

namespace Hyper.NodeServices.CommandModules.TestCommands
{
    internal class SuperLongRunningCommandTest : ICommandModule
    {
        public ICommandResponse Execute(ICommandExecutionContext context)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            while (stopwatch.Elapsed <= TimeSpan.FromMinutes(1) && !context.Token.IsCancellationRequested)
            {
                context.Activity.TrackFormat("Elapsed time: {0}", stopwatch.Elapsed);
                Thread.Sleep(TimeSpan.FromSeconds(5));
            }

            stopwatch.Stop();

            context.Activity.TrackFormat("Out of loop at {0}. Cancellation was{1} requested.", stopwatch.Elapsed, (context.Token.IsCancellationRequested ? "" : " not"));

            return new CommandResponse((context.Token.IsCancellationRequested ? MessageProcessStatusFlags.Cancelled : MessageProcessStatusFlags.Success));
        }
    }
}
