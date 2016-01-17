using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hyper.NodeServices.CommandModules;
using Hyper.NodeServices.CommandModules.SystemCommands;
using Hyper.NodeServices.Contracts.Extensibility.CommandModules;
using Hyper.NodeServices.Extensibility;
using Hyper.NodeServices.Extensibility.ActivityTracking;
using Hyper.NodeServices.Extensibility.CommandModules;
using Hyper.NodeServices.Extensibility.Configuration;
using Hyper.NodeServices.Extensibility.Configuration.Validation;
using Hyper.NodeServices.Extensibility.EventTracking;
using Hyper.NodeServices.SystemCommands.Contracts;
using Hyper.NodeServices.TaskIdProviders;

namespace Hyper.NodeServices
{
    public partial class HyperNodeService
    {
        #region Defaults

        private static readonly IHyperNodeEventHandler DefaultEventHandler = new HyperNodeEventHandlerBase();
        private static readonly ITaskIdProvider DefaultTaskIdProvider = new GuidTaskIdProvider();
        internal const bool DefaultTaskProgressCacheEnabled = false;
        internal const bool DefaultDiagnosticsEnabled = false;
        internal const int DefaultProgressCacheDurationMinutes = 60;
        internal const int DefaultMaxConcurrentTasks = -1;

        #endregion Defaults

        #region Configuration

        private static HyperNodeService Create(IHyperNodeConfigurationProvider configProvider)
        {
            if (configProvider == null)
                throw new ArgumentNullException("configProvider");

            IHyperNodeConfiguration config;

            try
            {
                config = configProvider.GetConfiguration();
            }
            catch (Exception ex)
            {
                throw new HyperNodeConfigurationException(
                    string.Format(
                        "An exception was thrown while attempting to retrieve the configuration for this {0} using {1}. See inner exception for details.",
                        typeof(HyperNodeService).FullName,
                        configProvider.GetType().FullName
                    ),
                    ex
                );
            }

            // Validate our configuration
            var builder = new StringBuilder();
            new HyperNodeConfigurationValidator(
                (sender, args) => builder.AppendLine(args.Message)
            ).ValidateConfiguration(config);

            // Check for validation errors before proceeding to create and configure our HyperNodeService instance
            if (builder.Length > 0)
            { throw new HyperNodeConfigurationException(builder.ToString()); }

            var service = new HyperNodeService(config.HyperNodeName)
            {
                EnableTaskProgressCache = config.EnableTaskProgressCache ?? DefaultTaskProgressCacheEnabled,
                EnableDiagnostics = config.EnableDiagnostics ?? DefaultDiagnosticsEnabled,
                TaskProgressCacheDuration = TimeSpan.FromMinutes(config.TaskProgressCacheDurationMinutes ?? DefaultProgressCacheDurationMinutes),
                MaxConcurrentTasks = config.MaxConcurrentTasks ?? DefaultMaxConcurrentTasks
            };

            ConfigureSystemCommands(service, config);
            ConfigureTaskProvider(service, config);
            ConfigureActivityMonitors(service, config);
            ConfigureCommandModules(service, config);
            ConfigureHyperNodeEventTracker(service, config);

            return service;
        }

        private static void ConfigureSystemCommands(HyperNodeService service, IHyperNodeConfiguration config)
        {
            // Grab our user-defined default for system commands being enabled or disabled
            bool? userDefinedSystemCommandsEnabledDefault = null;
            var systemCommandsCollection = config.SystemCommands;
            if (systemCommandsCollection != null)
                userDefinedSystemCommandsEnabledDefault = systemCommandsCollection.Enabled;

            // If the user didn't configure the system commands, they will be on by default (so that we can get task statuses and such)
            var actualDefaultEnabled = userDefinedSystemCommandsEnabledDefault ?? true;

            // Make all commands enabled or disabled according to the user-defined default, or the HyperNode's default if the user did not define a default
            var systemCommandConfigs = new List<CommandModuleConfiguration>
            {
                new CommandModuleConfiguration
                {
                    CommandName = SystemCommandName.GetCachedTaskProgressInfo,
                    Enabled = actualDefaultEnabled,
                    CommandModuleType = typeof(GetCachedTaskProgressInfoCommand)
                },
                new CommandModuleConfiguration
                {
                    CommandName = SystemCommandName.GetNodeStatus,
                    Enabled = actualDefaultEnabled,
                    CommandModuleType = typeof(GetNodeStatusCommand)
                },
                new CommandModuleConfiguration
                {
                    CommandName = SystemCommandName.GetChildNodes,
                    Enabled = actualDefaultEnabled,
                    CommandModuleType = typeof(GetChildNodesCommand)
                },
                new CommandModuleConfiguration
                {
                    CommandName = SystemCommandName.Discover,
                    Enabled = actualDefaultEnabled,
                    CommandModuleType = typeof(DiscoverCommand)
                },
                new CommandModuleConfiguration
                {
                    CommandName = SystemCommandName.Echo,
                    Enabled = actualDefaultEnabled,
                    CommandModuleType = typeof(EchoCommand)
                },
                new CommandModuleConfiguration
                {
                    CommandName = SystemCommandName.EnableCommand,
                    Enabled = actualDefaultEnabled,
                    CommandModuleType = typeof(EnableCommandModuleCommand)
                },
                new CommandModuleConfiguration
                {
                    CommandName = SystemCommandName.EnableActivityMonitor,
                    Enabled = actualDefaultEnabled,
                    CommandModuleType = typeof(EnableActivityMonitorCommand)
                },
                new CommandModuleConfiguration
                {
                    CommandName = SystemCommandName.RenameActivityMonitor,
                    Enabled = actualDefaultEnabled,
                    CommandModuleType = typeof(RenameActivityMonitorCommand)
                },
                new CommandModuleConfiguration
                {
                    CommandName = SystemCommandName.EnableTaskProgressCache,
                    Enabled = actualDefaultEnabled,
                    CommandModuleType = typeof(EnableTaskProgressCacheCommand)
                },
                new CommandModuleConfiguration
                {
                    CommandName = SystemCommandName.EnableDiagnostics,
                    Enabled = actualDefaultEnabled,
                    CommandModuleType = typeof(EnableDiagnosticsCommand)
                },
                new CommandModuleConfiguration
                {
                    CommandName = SystemCommandName.CancelTask,
                    Enabled = actualDefaultEnabled,
                    CommandModuleType = typeof(CancelTaskCommand)
                },
                new CommandModuleConfiguration
                {
                    CommandName = SystemCommandName.SetTaskProgressCacheDuration,
                    Enabled = actualDefaultEnabled,
                    CommandModuleType = typeof(SetTaskProgressCacheDurationCommand)
                }
            };

            foreach (var systemCommandConfig in systemCommandConfigs)
            {
                // Allow each system command to be enabled or disabled individually. This takes precedence over any defaults defined previously
                if (config.SystemCommands != null && config.SystemCommands.ContainsCommandName(systemCommandConfig.CommandName))
                {
                    var userConfig = config.SystemCommands.GetByCommandName(systemCommandConfig.CommandName);
                    if (userConfig != null)
                        systemCommandConfig.Enabled = userConfig.Enabled;
                }

                // Finally, try to add this system command to our collection
                service.AddCommandModuleConfiguration(systemCommandConfig);
            }
        }

        private static void ConfigureTaskProvider(HyperNodeService service, IHyperNodeConfiguration config)
        {
            ITaskIdProvider taskIdProvider = null;

            // Set our task id provider if applicable, but if we have any problems creating the instance or casting to ITaskIdProvider, we deliberately want to fail out and make them fix the configuration
            if (!string.IsNullOrWhiteSpace(config.TaskIdProviderType))
            {
                taskIdProvider = (ITaskIdProvider)Activator.CreateInstance(Type.GetType(config.TaskIdProviderType, true));
                taskIdProvider.Initialize();
            }

            service.TaskIdProvider = taskIdProvider ?? DefaultTaskIdProvider;
        }

        private static void ConfigureActivityMonitors(HyperNodeService service, IHyperNodeConfiguration config)
        {
            // Consider a null collection equivalent to an empty one
            if (config.ActivityMonitors == null)
                return;

            // Instantiate our activity monitors
            foreach (var monitorConfig in config.ActivityMonitors)
            {
                // If we have any problems creating the instance or casting to HyperNodeServiceActivityMonitor, we deliberately want to fail out and make them fix the config
                var monitor = (HyperNodeServiceActivityMonitor)Activator.CreateInstance(Type.GetType(monitorConfig.MonitorType, true));
                if (monitor != null)
                {
                    monitor.Name = monitorConfig.MonitorName;
                    monitor.Enabled = monitorConfig.Enabled;

                    monitor.Initialize();

                    if (service._customActivityMonitors.Any(m => m.Name == monitorConfig.MonitorName))
                    {
                        throw new DuplicateActivityMonitorException(
                            string.Format("An activity monitor already exists with the name '{0}'.", monitorConfig.MonitorName)
                        );
                    }

                    service._customActivityMonitors.Add(monitor);
                }
            }
        }

        private static void ConfigureCommandModules(HyperNodeService service, IHyperNodeConfiguration config)
        {
            // Consider a null collection equivalent to an empty one
            if (config.CommandModules == null)
                return;

            Type collectionRequestSerializerType = null;
            Type collectionResponseSerializerType = null;

            // First, see if we have any serializer types defined at the collection level
            if (!string.IsNullOrWhiteSpace(config.CommandModules.RequestSerializerType))
                collectionRequestSerializerType = Type.GetType(config.CommandModules.RequestSerializerType, true);
            if (!string.IsNullOrWhiteSpace(config.CommandModules.ResponseSerializerType))
                collectionResponseSerializerType = Type.GetType(config.CommandModules.ResponseSerializerType, true);

            foreach (var commandModuleConfig in config.CommandModules)
            {
                var commandModuleType = Type.GetType(commandModuleConfig.CommandModuleType, true);
                if (commandModuleType.GetInterfaces().Contains(typeof(ICommandModule)))
                {
                    Type commandRequestSerializerType = null;
                    Type commandResponseSerializerType = null;

                    // Now check to see if we have any serializer types defined at the command level
                    if (!string.IsNullOrWhiteSpace(commandModuleConfig.RequestSerializerType))
                        commandRequestSerializerType = Type.GetType(commandModuleConfig.RequestSerializerType, true);
                    if (!string.IsNullOrWhiteSpace(commandModuleConfig.ResponseSerializerType))
                        commandResponseSerializerType = Type.GetType(commandModuleConfig.ResponseSerializerType, true);

                    // Our final configuration allows command-level serializer types to take precedence, if available. Otherwise, the collection-level types are used.
                    var configRequestSerializerType = commandRequestSerializerType ?? collectionRequestSerializerType;
                    var configResponseSerializerType = commandResponseSerializerType ?? collectionResponseSerializerType;

                    ICommandRequestSerializer configRequestSerializer = null;
                    ICommandResponseSerializer configResponseSerializer = null;

                    // Attempt construction of config-level serializer types
                    if (configRequestSerializerType != null)
                        configRequestSerializer = (ICommandRequestSerializer)Activator.CreateInstance(configRequestSerializerType);
                    if (configResponseSerializerType != null)
                        configResponseSerializer = (ICommandResponseSerializer)Activator.CreateInstance(configResponseSerializerType);

                    // Finally, construct our command module configuration
                    var commandConfig = new CommandModuleConfiguration
                    {
                        CommandName = commandModuleConfig.CommandName,
                        Enabled = commandModuleConfig.Enabled,
                        CommandModuleType = commandModuleType,
                        RequestSerializer = configRequestSerializer ?? DefaultRequestSerializer,
                        ResponseSerializer = configResponseSerializer ?? DefaultResponseSerializer
                    };

                    service.AddCommandModuleConfiguration(commandConfig);
                }
            }
        }

        private static void ConfigureHyperNodeEventTracker(HyperNodeService service, IHyperNodeConfiguration config)
        {
            IHyperNodeEventHandler eventHandler = null;

            // Set our event handler if applicable, but if we have any problems creating the instance or casting to HyperNodeEventHandlerBase, we deliberately want to fail out and make them fix the configuration
            if (!string.IsNullOrWhiteSpace(config.HyperNodeEventHandlerType))
            {
                eventHandler = (IHyperNodeEventHandler)Activator.CreateInstance(Type.GetType(config.HyperNodeEventHandlerType, true));
                eventHandler.Initialize();
            }

            service.EventHandler = eventHandler ?? DefaultEventHandler;
        }
        
        #endregion Configuration
    }
}
