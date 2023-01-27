using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Contracts.Extensibility.CommandModules;
using Hyper.NodeServices.Contracts.Extensibility.Serializers;
using Hyper.NodeServices.Extensibility;
using Hyper.NodeServices.Extensibility.CommandModules;
using Hyper.NodeServices.UnitTesting.Contracts.CommandModules;

namespace Hyper.NodeServices.CommandModules.UnitTestingCommands
{
    /// <summary>
    /// Unit test for long-running awaitable command modules.
    /// </summary>
    public class LongRunningAwaitableCommandTest : IAwaitableCommandModule
    {
        private static readonly TimeSpan DefaultTotalRunTime = TimeSpan.FromSeconds(30);
        private static readonly TimeSpan DefaultMinimumSleepInterval = TimeSpan.FromSeconds(1);
        private static readonly TimeSpan DefaultMaximumSleepInterval = TimeSpan.FromSeconds(5);

        /// <summary>
        /// Executes a long-running command using the specified <see cref="ICommandExecutionContext"/>.
        /// </summary>
        /// <param name="context">The <see cref="ICommandExecutionContext"/> to use to run this command module.</param>
        /// <returns></returns>
        public async Task<ICommandResponse> Execute(ICommandExecutionContext context)
        {
            // This technique allows us to optionally take a command request. If they just want to run the default settings, they can just pass an empty string and we'll supply the default values.
            if (!(context.Request is CommandRequestString stringRequest))
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

                try
                {
                    await Task.Delay(sleepMilliseconds, context.Token);

                    // Avoid reporting more than 100% or reporting 100% multiple times
                    if (stopwatch.ElapsedMilliseconds < totalRunTime.TotalMilliseconds)
                    {
                        context.Activity.Track(
                            $"Progress update {++progressReportCount}.",
                            stopwatch.ElapsedMilliseconds,
                            totalRunTime.TotalMilliseconds
                        );
                    }
                }
                catch (OperationCanceledException)
                {
                    break; // break on cancel
                }
            }

            stopwatch.Stop();

            if (context.Token.IsCancellationRequested)
            {
                context.Activity.Track($"Cancellation requested after {stopwatch.Elapsed}.");
            }
            else
            {
                // Otherwise, we completed successfully, so show 100%
                context.Activity.Track(
                    $"Progress update {++progressReportCount}.",
                    totalRunTime.TotalMilliseconds,
                    totalRunTime.TotalMilliseconds
                );    
            }

            return new CommandResponse(MessageProcessStatusFlags.Success);
        }
    }
}
