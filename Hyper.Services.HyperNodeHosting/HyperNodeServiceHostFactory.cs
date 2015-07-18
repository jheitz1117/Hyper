using System;
using System.Configuration;
using System.ServiceModel;
using Hyper.Services.HyperNodeServices;
using Hyper.Services.HyperNodeServices.Configuration;
using Hyper.WcfHosting;

namespace Hyper.Services.HyperNodeHosting
{
    public class HyperNodeServiceHostFactory : IServiceHostFactory
    {
        public ServiceHost Create()
        {
            var config = (HyperNodeConfigurationSection)ConfigurationManager.GetSection("hyperNet/hyperNode");

            var service = new HyperNodeService(config.HyperNodeName)
            {
                EnableActivityCache = config.EnableActivityCache,
                ActivityCacheSlidingExpiration = TimeSpan.FromMinutes(config.ActivityCacheSlidingExpirationMinutes)
            };

            // TODO: Read the app.config and construct any applicable custom task ID provider
            var taskIdProviderTypeString = "";
            if (!string.IsNullOrWhiteSpace(taskIdProviderTypeString))
            {
                var taskIdProvider = Activator.CreateInstance(Type.GetType(taskIdProviderTypeString, true)) as ITaskIdProvider;
                service.TaskIdProvider = taskIdProvider;
            }

            foreach (var monitorConfig in config.ActivityMonitors)
            {
                // If we have any problems creating the instance or casting to HyperNodeServiceActivityMonitor, we deliberately want to fail out and make them fix the app.config
                var monitor = (HyperNodeServiceActivityMonitor)Activator.CreateInstance(Type.GetType(monitorConfig.Type, true));
                if (monitor != null)
                {
                    monitor.Name = monitorConfig.Name;
                    monitor.Enabled = monitorConfig.Enabled;

                    service.AddActivityMonitor(monitor);
                }
            }
            
            return new ServiceHost(service);
        }
    }
}
