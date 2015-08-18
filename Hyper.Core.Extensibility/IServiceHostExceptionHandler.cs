using System;
using System.ServiceModel;

namespace Hyper.Extensibility.WcfHosting
{
    /// <summary>
    /// Defines a method to handle exceptions thrown by a <see cref="ServiceHost"/> instance.
    /// </summary>
    public interface IServiceHostExceptionHandler
    {
        /// <summary>
        /// Handles the specified <see cref="Exception"/>.
        /// </summary>
        /// <param name="ex">The <see cref="Exception"/> to handle.</param>
        void HandleException(Exception ex);
    }
}
