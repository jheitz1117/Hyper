using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Contracts.Extensibility;
using Hyper.NodeServices.Contracts.Serializers;
using Hyper.NodeServices.Extensibility.CommandModules;
using Hyper.NodeServices.Extensibility.Exceptions;
using Hyper.NodeServices.UnitTesting.Contracts.CommandModules;

namespace Hyper.NodeServices.CommandModules.UnitTestingCommands
{
    internal class LongRunningCommandTest : ICommandModule
    {
        private static readonly TimeSpan DefaultTotalRunTime = TimeSpan.FromSeconds(30);
        private static readonly TimeSpan DefaultMinimumSleepInterval = TimeSpan.FromSeconds(1);
        private static readonly TimeSpan DefaultMaximumSleepInterval = TimeSpan.FromSeconds(5);

        public ICommandResponse Execute(ICommandExecutionContext context)
        {
            // This technique allows us to optionally take a command request. If they just want to run the default settings, they can just pass an empty string and we'll supply the default values.
            var stringRequest = context.Request as CommandRequestString;
            if (stringRequest == null)
                throw new InvalidCommandRequestTypeException(typeof(CommandRequestString), context.Request.GetType());

            // Test if we have a command request string. If we have a non-blank request string, then try to deserialize it
            var request = new LongRunningCommandTestRequest();
            if (!string.IsNullOrWhiteSpace(stringRequest.RequestString))
            {
                context.Activity.Track("Request string found. Attempting deserialization of request object.");
                request = ((ICommandRequestSerializer)new NetDataContractRequestSerializer<LongRunningCommandTestRequest>()).Deserialize(stringRequest.RequestString) as LongRunningCommandTestRequest;

                // Now that we've tried to deserialize it manually, if the request couldn't be deserialized into the expected, we can throw the invalid command request type exception.
                if (request == null)
                    throw new InvalidCommandRequestTypeException(typeof(LongRunningCommandTestRequest),
                        context.Request.GetType());
            }

            var totalRunTime = request.TotalRunTime ?? DefaultTotalRunTime;
            var minSleepMilliseconds = (int)Math.Min(int.MaxValue, (request.MinimumSleepInterval ?? DefaultMinimumSleepInterval).TotalMilliseconds);
            var maxSleepMilliseconds = (int)Math.Min(int.MaxValue, (request.MaximumSleepInterval ?? DefaultMaximumSleepInterval).TotalMilliseconds);

            var stopwatch = new Stopwatch();
            var rand = new Random();
            var progressReportCount = 0;

            context.Activity.Track("Starting long-running command test.");
            stopwatch.Start();
            while (stopwatch.ElapsedMilliseconds < totalRunTime.TotalMilliseconds && !context.Token.IsCancellationRequested)
            {
                // Capture the remaining run time. Smart check for max size for an Int32.
                var remainingMilliseconds = (int)Math.Min(int.MaxValue, totalRunTime.TotalMilliseconds - stopwatch.ElapsedMilliseconds);

                // Try to sleep for at least the minimum time span unless there is less time remaining. Don't sleep for more than the maximum time span
                var sleepMilliseconds = rand.Next(
                    Math.Min(minSleepMilliseconds, remainingMilliseconds),
                    Math.Min(maxSleepMilliseconds, remainingMilliseconds)
                );

                Task.WaitAll(
                    Task.Delay(sleepMilliseconds, context.Token)
                );

                // Avoid reporting more than 100% or reporting 100% multiple times
                if (stopwatch.ElapsedMilliseconds < totalRunTime.TotalMilliseconds)
                {
                    context.Activity.Track(
                        string.Format("Progress update {0}.", ++progressReportCount),
                        stopwatch.ElapsedMilliseconds, 
                        totalRunTime.TotalMilliseconds
                    );
                }
            }

            stopwatch.Stop();

            if (context.Token.IsCancellationRequested)
            {
                context.Activity.TrackFormat("Cancellation requested after {0}.", stopwatch.Elapsed);
            }
            else
            {
                context.Activity.Track(
                    string.Format("Progress update {0}.", ++progressReportCount),
                    totalRunTime.TotalMilliseconds,
                    totalRunTime.TotalMilliseconds
                );    
            }

            return new CommandResponse(MessageProcessStatusFlags.Success);
        }
    }
}
