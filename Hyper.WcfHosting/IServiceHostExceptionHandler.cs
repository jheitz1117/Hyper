using System;

namespace Hyper.WcfHosting
{
    public interface IServiceHostExceptionHandler
    {
        void HandleException(Exception ex);
    }
}
