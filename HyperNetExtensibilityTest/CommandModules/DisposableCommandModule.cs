using System;
using System.Diagnostics;
using Hyper.Services.HyperNodeContracts;
using Hyper.Services.HyperNodeExtensibility;

namespace HyperNetExtensibilityTest.CommandModules
{
    public class DisposableCommandModule : ICommandModule, IDisposable
    {
        public bool IsDisposed { get; private set; }

        public ICommandModuleResponse Execute(ICommandExecutionContext context)
        {
            context.Activity.Track("Executing DisposableCommandModule.");
            
            return new CommandResponse(MessageProcessStatusFlags.Success);
        }

        public void Dispose()
        {
            Trace.WriteLine("Disposing of DisposableCommandModule.");
            this.IsDisposed = true;
        }
    }
}
