using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading;

namespace Hyper.WcfHosting
{
    /// <summary>
    /// <see cref="ServiceHost"/> implementation that supports cancellation.
    /// </summary>
    public sealed class CancellableServiceHost : ServiceHost
    {
        private readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();
        private readonly List<CancellationTokenRegistration> _cancellationCallbackRegistrations = new List<CancellationTokenRegistration>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CancellableServiceHost"/> class with the type of service and its base addresses specified.
        /// </summary>
        /// <param name="type">The type of hosted service.</param>
        /// <param name="baseAddresses">An array of type <see cref="Uri"/> that contains the base addresses for the hosted service.</param>
        public CancellableServiceHost(Type type, params Uri[] baseAddresses)
            : base(type, baseAddresses) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="CancellableServiceHost"/> class with the instance of the service and its base addresses specified.
        /// </summary>
        /// <param name="singletonInstance">The instance of the hosted service.</param>
        /// <param name="baseAddresses">An array of type <see cref="Uri"/> that contains the base addresses for the hosted service.</param>
        public CancellableServiceHost(object singletonInstance, params Uri[] baseAddresses)
            : base(singletonInstance, baseAddresses) { }

        /// <summary>
        /// Registers a delegate that will be called when this <see cref="CancellationToken"/> is canceled.
        /// </summary>
        /// <param name="callback">The delegate to be executed when the <see cref="CancellationToken"/> is canceled.</param>
        public void RegisterCancellationDelegate(Action callback)
        {
            _cancellationCallbackRegistrations.Add(
                _tokenSource.Token.Register(callback)
            );
        }

        /// <summary>
        /// Registers a delegate that will be called when this <see cref="CancellationToken"/> is canceled.
        /// </summary>
        /// <param name="callback">The delegate to be executed when the <see cref="CancellationToken"/> is canceled.</param>
        /// <param name="useSynchronizationContext">A Boolean value that indicates whether to capture the current <see cref="SynchronizationContext"/> and use it when invoking the <paramref name="callback"/>.</param>
        public void RegisterCancellationDelegate(Action callback, bool useSynchronizationContext)
        {
            _cancellationCallbackRegistrations.Add(
                _tokenSource.Token.Register(callback, useSynchronizationContext)
            );
        }

        /// <summary>
        /// Registers a delegate that will be called when this <see cref="CancellationToken"/> is canceled.
        /// </summary>
        /// <param name="callback">The delegate to be executed when the <see cref="CancellationToken"/> is canceled.</param>
        /// <param name="state">The state to pass to the <paramref name="callback"/> when the delegate is invoked. This may be null.</param>
        public void RegisterCancellationDelegate(Action<object> callback, object state)
        {
            _cancellationCallbackRegistrations.Add(
                _tokenSource.Token.Register(callback, state)
            );
        }

        /// <summary>
        /// Registers a delegate that will be called when this <see cref="CancellationToken"/> is canceled.
        /// </summary>
        /// <param name="callback">The delegate to be executed when the <see cref="CancellationToken"/> is canceled.</param>
        /// <param name="state">The state to pass to the <paramref name="callback"/> when the delegate is invoked. This may be null.</param>
        /// <param name="useSynchronizationContext">A Boolean value that indicates whether to capture the current <see cref="SynchronizationContext"/> and use it when invoking the <paramref name="callback"/>.</param>
        public void RegisterCancellationDelegate(Action<object> callback, object state, bool useSynchronizationContext)
        {
            _cancellationCallbackRegistrations.Add(
                _tokenSource.Token.Register(callback, state, useSynchronizationContext)
            );
        }

        /// <summary>
        /// Aborts the service.
        /// </summary>
        protected override void OnAbort()
        {
            _tokenSource.Cancel();

            base.OnAbort();
        }

        /// <summary>
        /// Invoked during the transition of a communication object into the closing state.
        /// </summary>
        protected override void OnClosing()
        {
            _tokenSource.Cancel();

            base.OnClosing();
        }

        /// <summary>
        /// Disposes of disposable services that are being hosted when the service host is closed.
        /// </summary>
        protected override void OnClosed()
        {
            try
            {
                // Dispose all of our callback registrations first
                foreach (var registration in _cancellationCallbackRegistrations)
                {
                    registration.Dispose();
                }

                // Now finally dipose our token source
                _tokenSource.Dispose();
            }
            finally
            {
                base.OnClosed();
            }
        }
    }
}
