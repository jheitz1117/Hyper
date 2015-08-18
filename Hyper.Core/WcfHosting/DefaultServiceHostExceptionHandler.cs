using System;
using System.Diagnostics;
using Hyper.Extensibility.WcfHosting;

namespace Hyper.WcfHosting
{
    /// <summary>
    /// Default implementation of <see cref="IServiceHostExceptionHandler"/> that writes exceptions to the <see cref="Trace"/>.
    /// </summary>
    public sealed class DefaultServiceHostExceptionHandler : IServiceHostExceptionHandler
    {
        /// <summary>
        /// Calls <see cref="Exception.ToString()"/> on the specified <see cref="Exception"/> and writes the results to <see cref="Trace"/>.<see cref="Trace.WriteLine(object)"/>.
        /// </summary>
        /// <param name="ex">The <see cref="Exception"/> to trace.</param>
        public void HandleException(Exception ex)
        {
            Trace.WriteLine(ex);
        }
    }
}
