using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using Hyper.Extensibility.WcfHosting;

namespace Hyper.WcfHosting
{
    /// <summary>
    /// Simplifies self-hosting for <see cref="ServiceHost"/> objects.
    /// Supports singleton service contract implementations that implement <see cref="IDisposable"/>.
    /// </summary>
    public sealed class HyperServiceHostContainer
    {
        private ServiceHost _host;

        private readonly IServiceHostFactory _hostFactory;
        private readonly IServiceHostExceptionHandler _timeoutExceptionHandler;
        private readonly IServiceHostExceptionHandler _communicationExceptionHandler;
        private readonly IServiceHostExceptionHandler _genericExceptionHandler;

        /// <summary>
        /// List of endpoints on which the <see cref="ServiceHost"/> is listening.
        /// </summary>
        public ServiceEndpointCollection Endpoints => _host?.Description?.Endpoints;

        /// <summary>
        /// Indiciates whether the internal <see cref="ServiceHost"/> has been created and is in the <see cref="CommunicationState.Opened"/> state.
        /// </summary>
        public bool IsRunning => _host != null && _host.State == CommunicationState.Opened;

        /// <summary>
        /// Indicates whether the <see cref="Stop()"/> method will try to dispose the service hosted by
        /// the internal <see cref="ServiceHost"/>.
        /// </summary>
        public bool DisposeServiceOnStop { get; set; }

        /// <summary>
        /// Initializes an instance of <see cref="HyperServiceHostContainer"/> with the specified factory method and <see cref="IServiceHostExceptionHandler"/> implementation.
        /// </summary>
        /// <param name="factory">The delegate that is invoked to create the <see cref="ServiceHost"/> object to wrap.</param>
        /// <param name="exceptionHandler">The <see cref="IServiceHostExceptionHandler"/> implementation to use when any is thrown.</param>
        public HyperServiceHostContainer(Func<ServiceHost> factory, IServiceHostExceptionHandler exceptionHandler)
            : this(factory, exceptionHandler, exceptionHandler, exceptionHandler)
        { }

        /// <summary>
        /// Initializes an instance of <see cref="HyperServiceHostContainer"/> with the specified factory method and <see cref="IServiceHostExceptionHandler"/> implementations.
        /// </summary>
        /// <param name="factory">The delegate that is invoked to create the <see cref="ServiceHost"/> object to wrap.</param>
        /// <param name="timeoutExceptionHandler">The <see cref="IServiceHostExceptionHandler"/> implementation to use when a <see cref="TimeoutException"/> is thrown.</param>
        /// <param name="communicationExceptionHandler">The <see cref="IServiceHostExceptionHandler"/> implementation to use when a <see cref="CommunicationException"/> is thrown.</param>
        /// <param name="genericExceptionHandler">The <see cref="IServiceHostExceptionHandler"/> implementation to use when an <see cref="Exception"/> is thrown that is not a <see cref="TimeoutException"/> or a <see cref="CommunicationException"/>.</param>
        public HyperServiceHostContainer(Func<ServiceHost> factory,
                           IServiceHostExceptionHandler timeoutExceptionHandler,
                           IServiceHostExceptionHandler communicationExceptionHandler,
                           IServiceHostExceptionHandler genericExceptionHandler)
            : this(new ServiceHostFactoryMethodWrapper(factory), timeoutExceptionHandler, communicationExceptionHandler, genericExceptionHandler)
        { }

        /// <summary>
        /// Initializes an instance of <see cref="HyperServiceHostContainer"/> with the specified <see cref="IServiceHostFactory"/> and <see cref="IServiceHostExceptionHandler"/> implementations.
        /// </summary>
        /// <param name="hostFactory">The <see cref="IServiceHostFactory"/> that is used to create the <see cref="ServiceHost"/> object to wrap.</param>
        /// <param name="exceptionHandler">The <see cref="IServiceHostExceptionHandler"/> implementation to use when any is thrown.</param>
        public HyperServiceHostContainer(IServiceHostFactory hostFactory, IServiceHostExceptionHandler exceptionHandler)
            : this(hostFactory, exceptionHandler, exceptionHandler, exceptionHandler)
        { }

        /// <summary>
        /// Initializes an instance of <see cref="HyperServiceHostContainer"/> with the specified <see cref="IServiceHostFactory"/> and <see cref="IServiceHostExceptionHandler"/> implementations.
        /// </summary>
        /// <param name="hostFactory">The <see cref="IServiceHostFactory"/> that is used to create the <see cref="ServiceHost"/> object to wrap.</param>
        /// <param name="timeoutExceptionHandler">The <see cref="IServiceHostExceptionHandler"/> implementation to use when a <see cref="TimeoutException"/> is thrown.</param>
        /// <param name="communicationExceptionHandler">The <see cref="IServiceHostExceptionHandler"/> implementation to use when a <see cref="CommunicationException"/> is thrown.</param>
        /// <param name="genericExceptionHandler">The <see cref="IServiceHostExceptionHandler"/> implementation to use when an <see cref="Exception"/> is thrown that is not a <see cref="TimeoutException"/> or a <see cref="CommunicationException"/>.</param>
        public HyperServiceHostContainer(IServiceHostFactory hostFactory, 
                           IServiceHostExceptionHandler timeoutExceptionHandler,
                           IServiceHostExceptionHandler communicationExceptionHandler,
                           IServiceHostExceptionHandler genericExceptionHandler)
        {
            if (hostFactory == null)
                throw new ArgumentNullException(nameof(hostFactory));
            if (timeoutExceptionHandler == null)
                throw new ArgumentNullException(nameof(timeoutExceptionHandler));
            if (communicationExceptionHandler == null)
                throw new ArgumentNullException(nameof(communicationExceptionHandler));
            if (genericExceptionHandler == null)
                throw new ArgumentNullException(nameof(genericExceptionHandler));

            _hostFactory = hostFactory;
            _timeoutExceptionHandler = timeoutExceptionHandler;
            _communicationExceptionHandler = communicationExceptionHandler;
            _genericExceptionHandler = genericExceptionHandler;

            // By default, this property is true, making the container a one stop shop for ServiceHost management. Users can turn the feature off if they have to though.
            DisposeServiceOnStop = true;
        }

        /// <summary>
        /// Creates the <see cref="ServiceHost"/> if it does not exist and calls its Open() method. Exception handling is
        /// delegated to the <see cref="IServiceHostExceptionHandler"/> implementations specified in the constructor.
        /// </summary>
        /// <returns></returns>
        public bool Start()
        {
            var startedSuccessfully = false;
            try
            {
                if (_host == null)
                    _host = _hostFactory.Create();
                
                _host.Open();
                startedSuccessfully = true;
            }
            catch (TimeoutException exTimeout)
            {
                if (_timeoutExceptionHandler != null)
                    _timeoutExceptionHandler.HandleException(exTimeout);
                else throw;
            }
            catch (CommunicationException exCommunication)
            {
                if (_communicationExceptionHandler != null)
                    _communicationExceptionHandler.HandleException(exCommunication);
                else throw;
            }
            catch (Exception ex)
            {
                if (_genericExceptionHandler != null)
                    _genericExceptionHandler.HandleException(ex);
                else throw;
            }

            return startedSuccessfully;
        }

        /// <summary>
        /// Calls the <see cref="CommunicationObject.Abort()"/> method if it is in the <see cref="CommunicationState.Faulted"/> state.
        /// Otherwise, calls the <see cref="CommunicationObject.Close()"/> method instead.
        /// Calls <see cref="IDisposable.Dispose()"/> on the hosted service if it implements <see cref="IDisposable"/>.
        /// Exception handling is delegated to the <see cref="IServiceHostExceptionHandler"/> implementations specified in the constructor.
        /// </summary>
        public void Stop()
        {
            IDisposable disposableService = null;

            try
            {
                if (_host != null)
                {
                    disposableService = _host.SingletonInstance as IDisposable;
                    if (_host.State == CommunicationState.Faulted)
                        _host.Abort();
                    else
                        _host.Close();

                    _host = null;
                }
            }
            catch (TimeoutException exTimeout)
            {
                _timeoutExceptionHandler?.HandleException(exTimeout);
            }
            catch (CommunicationException exCommunication)
            {
                _communicationExceptionHandler?.HandleException(exCommunication);
            }
            catch (Exception ex)
            {
                _genericExceptionHandler?.HandleException(ex);
            }
            finally
            {
                // Dispose of our service if applicable
                if (disposableService != null && DisposeServiceOnStop)
                    disposableService.Dispose();
            }
        }
    }
}
