using System;
using System.ServiceModel;
using System.Threading;

namespace Hyper.WcfHosting
{
    public sealed class CancellableServiceHost : ServiceHost
    {
        private readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();

        public CancellationToken Token
        {
            get
            {
                return _tokenSource.Token;
            }
        }

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
        /// Invoked during the transition of a communication object into the closing state.
        /// </summary>
        protected override void OnClosing()
        {
            _tokenSource.Cancel();

            base.OnClosing();
        }

        /// <summary>
        /// Aborts the service
        /// </summary>
        protected override void OnAbort()
        {
            _tokenSource.Cancel();

            base.OnAbort();
        }
    }
}
