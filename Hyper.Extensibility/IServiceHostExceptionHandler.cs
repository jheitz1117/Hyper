using System;

namespace Hyper.Extensibility.WcfHosting
{
    public interface IServiceHostExceptionHandler
    {
        void HandleException(Exception ex);
    }
}
