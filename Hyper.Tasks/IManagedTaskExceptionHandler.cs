using System;

namespace Hyper.Tasks
{
    public interface IManagedTaskExceptionHandler
    {
        void HandleException(Exception ex);
    }
}
