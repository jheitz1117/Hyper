﻿using System;
using System.ServiceModel;
using Hyper.Extensibility.WcfHosting;

namespace Hyper.WcfHosting
{
    internal class ServiceHostFactoryMethodWrapper : IServiceHostFactory
    {
        private readonly Func<ServiceHost> _factory;

        public ServiceHostFactoryMethodWrapper(Func<ServiceHost> factory)
        {
            _factory = factory;
        }

        public ServiceHost Create()
        {
            return _factory();
        }
    }
}