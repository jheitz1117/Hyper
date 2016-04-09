using System;
using System.Diagnostics;
using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Contracts.Extensibility.CommandModules;
using Hyper.NodeServices.Extensibility.CommandModules;

namespace NodeModuleTest.CommandModules
{
    public class DisposableCommandModule : ICommandModule, IDisposable
    {
        public bool IsDisposed { get; private set; }

        public ICommandResponse Execute(ICommandExecutionContext context)
        {
            context.Activity.Track("Executing DisposableCommandModule.");
            
            return new CommandResponse(MessageProcessStatusFlags.Success);
        }

        public void Dispose()
        {
            Trace.WriteLine("Disposing of DisposableCommandModule.");
            IsDisposed = true;
        }
    }
}
