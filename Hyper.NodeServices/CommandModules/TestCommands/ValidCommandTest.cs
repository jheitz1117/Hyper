using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Contracts.Extensibility;
using Hyper.NodeServices.Extensibility;

namespace Hyper.NodeServices.CommandModules.TestCommands
{
    internal class ValidCommandTest : ICommandModule
    {
        public ICommandResponse Execute(ICommandExecutionContext context)
        {
            // This simulates a long-running task
            //Thread.Sleep(10000);
            context.Activity.Track("Got here past the sleep!");

            //Thread.Sleep(3000);
            context.Activity.Track("Another step in the process.");
            //throw new Exception("Bad!");

            return new CommandResponse(MessageProcessStatusFlags.Success | MessageProcessStatusFlags.HadNonFatalErrors | MessageProcessStatusFlags.HadWarnings);
        }
    }
}
