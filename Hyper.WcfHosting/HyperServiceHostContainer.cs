using System;
using System.ServiceModel;
using System.ServiceModel.Description;
using Hyper.Extensibility.WcfHosting;

namespace Hyper.WcfHosting
{
    public sealed class HyperServiceHostContainer
    {
        private ServiceHost _host;

        private readonly IServiceHostFactory _hostFactory;
        private readonly IServiceHostExceptionHandler _timeoutExceptionHandler;
        private readonly IServiceHostExceptionHandler _communicationExceptionHandler;
        private readonly IServiceHostExceptionHandler _genericExceptionHandler;

        public HyperServiceHostContainer(Func<ServiceHost> factory, IServiceHostExceptionHandler exceptionHandler)
            : this(new ServiceHostFactoryMethodWrapper(factory), exceptionHandler, exceptionHandler, exceptionHandler)
        { }

        public HyperServiceHostContainer(IServiceHostFactory hostFactory, IServiceHostExceptionHandler exceptionHandler)
            : this(hostFactory, exceptionHandler, exceptionHandler, exceptionHandler)
        { }

        public HyperServiceHostContainer(IServiceHostFactory hostFactory, 
                           IServiceHostExceptionHandler timeoutExceptionHandler,
                           IServiceHostExceptionHandler communicationExceptionHandler,
                           IServiceHostExceptionHandler genericExceptionHandler)
        {
            if (hostFactory == null)
                throw new ArgumentNullException("hostFactory");
            if (timeoutExceptionHandler == null)
                throw new ArgumentNullException("timeoutExceptionHandler");
            if (communicationExceptionHandler == null)
                throw new ArgumentNullException("communicationExceptionHandler");
            if (genericExceptionHandler == null)
                throw new ArgumentNullException("genericExceptionHandler");

            _hostFactory = hostFactory;
            _timeoutExceptionHandler = timeoutExceptionHandler;
            _communicationExceptionHandler = communicationExceptionHandler;
            _genericExceptionHandler = genericExceptionHandler;
        }

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
                if (_timeoutExceptionHandler != null)
                    _timeoutExceptionHandler.HandleException(exTimeout);
            }
            catch (CommunicationException exCommunication)
            {
                if (_communicationExceptionHandler != null)
                    _communicationExceptionHandler.HandleException(exCommunication);
            }
            catch (Exception ex)
            {
                if (_genericExceptionHandler != null)
                    _genericExceptionHandler.HandleException(ex);
            }
            finally
            {
                // Always try to call Dispose() if our service was disposable
                if (disposableService != null)
                    disposableService.Dispose();
            }
        }

        public ServiceEndpointCollection Endpoints
        {
            get
            {
                if (_host != null && _host.Description != null && _host.Description.Endpoints != null)
                    return _host.Description.Endpoints;

                return null;
            }
        }
    }
}
