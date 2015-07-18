using System;
using System.Diagnostics;

namespace Hyper.WcfHosting
{
    public class DefaultServiceHostExceptionHandler : IServiceHostExceptionHandler
    {
        public void HandleException(Exception ex)
        {
            Trace.WriteLine(ex);
        }
    }
}
