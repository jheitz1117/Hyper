using System;
using System.Diagnostics;
using Hyper.Services.HyperNodeContracts;
using Hyper.Services.HyperNodeExtensibility;

namespace HyperNetExtensibilityTest.CommandModules
{
    public class DisposableCommandModule : SimpleCommandModule, IDisposable
    {
        public bool IsDisposed { get; private set; }

        public override ICommandModuleResponse Execute(ICommandExecutionContext context)
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
