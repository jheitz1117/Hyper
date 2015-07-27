using System;
using System.Diagnostics;
using Hyper.Extensibility.WcfHosting;

namespace Hyper.WcfHosting
{
    public sealed class DefaultServiceHostExceptionHandler : IServiceHostExceptionHandler
    {
        public void HandleException(Exception ex)
        {
            Trace.WriteLine(ex);
        }
    }
}
