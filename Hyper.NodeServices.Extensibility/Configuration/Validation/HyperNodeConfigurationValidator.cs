﻿using System;
using System.Linq;
using Hyper.NodeServices.Contracts.Extensibility.CommandModules;
using Hyper.NodeServices.Extensibility.ActivityTracking;
using Hyper.NodeServices.Extensibility.CommandModules;
using Hyper.NodeServices.Extensibility.EventTracking;
using Hyper.NodeServices.SystemCommands.Contracts;

// Keep in mind now that we're going to have two levels of validation (XSD and manual validation), we'll have to keep them in sync.
// The reason we can't simplify this is because of intellisense: We can't eliminate XSD because then we lose intellisense in the
// app.config. However, we can't eliminate validation in this provider class because we would lose support for the default values.

// To clarify about the default values, the default values used to be supplied by the configuration element classes that integrate
// with the app.config custom section. However, now that we're supporting multiple configuration strategies, we had to move the
// default values to a centralized location, i.e., this provider class.

// Finally, since we're doing validation here, we must check required values in addition to supplying default values since there's
// no guarantee the XSD was ever used to validate the input. We don't even know if the input was in XML format to begin with.

namespace Hyper.NodeServices.Extensibility.Configuration.Validation
{
    /// <summary>
    /// Represents the callback method that will handle configuration validation events and the <see cref="HyperNodeConfigurationValidationEventArgs"/>.
    /// </summary>
    public delegate void HyperNodeConfigurationValidationEventHandler(object sender, HyperNodeConfigurationValidationEventArgs e);

    /// <summary>
    /// Validates implementations of <see cref="IHyperNodeConfiguration"/>.
    /// </summary>
    public sealed class HyperNodeConfigurationValidator
    {
        private readonly HyperNodeConfigurationValidationEventHandler _validationEventHandler;

        #region Public Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="HyperNodeConfigurationValidator"/> class.
        /// </summary>
        /// <param name="validationEventHandler">Callback for validation errors. If this is null, an exception is thrown if any validation errors occur.</param>
        public HyperNodeConfigurationValidator(HyperNodeConfigurationValidationEventHandler validationEventHandler)
        {
            _validationEventHandler = validationEventHandler ?? DefaultValidationEventHandler;
        }

        /// <summary>
        /// Validates the specified <see cref="IHyperNodeConfiguration"/> object.
        /// </summary>
        /// <param name="config"><see cref="IHyperNodeConfiguration"/> object to validate.</param>
        public void ValidateConfiguration(IHyperNodeConfiguration config)
        {
            if (config == null)
            { throw new ArgumentNullException(nameof(config)); }

            var configClassName = config.GetType().FullName;
            if (string.IsNullOrWhiteSpace(config.HyperNodeName))
            {
                RaiseValidationEvent(
                    new HyperNodeConfigurationException($"The HyperNodeName property is required for {configClassName}.")
                );
            }

            // TaskIdProviderType is not required, but if it is specified, it must implement the correct interface
            if (!string.IsNullOrWhiteSpace(config.TaskIdProviderType))
                ValidateTypeImplementsInterface(config.TaskIdProviderType, typeof(ITaskIdProvider));

            // HyperNodeEventHandlerType is not required, but if it is specified, it must implement the correct interface
            if (!string.IsNullOrWhiteSpace(config.HyperNodeEventHandlerType))
                ValidateTypeImplementsInterface(config.HyperNodeEventHandlerType, typeof(IHyperNodeEventHandler));

            if (config.ActivityMonitors != null)
            {
                foreach (var activityMonitor in config.ActivityMonitors)
                {
                    ValidateConfiguration(activityMonitor);
                }
            }

            if (config.SystemCommands != null)
            {
                foreach (var systemCommand in config.SystemCommands)
                {
                    ValidateConfiguration(systemCommand);
                }
            }

            if (config.CommandModules != null)
            {
                // RequestSerializerType property is not required at the collection level, but if it is specified, it must implement the correct interface
                if (!string.IsNullOrWhiteSpace(config.CommandModules.RequestSerializerType))
                    ValidateTypeImplementsInterface(config.CommandModules.RequestSerializerType, typeof(ICommandRequestSerializer));

                // ResponseSerializerType property is not required at the collection level, but if it is specified, it must implement the correct interface
                if (!string.IsNullOrWhiteSpace(config.CommandModules.ResponseSerializerType))
                    ValidateTypeImplementsInterface(config.CommandModules.ResponseSerializerType, typeof(ICommandResponseSerializer));

                foreach (var commandModule in config.CommandModules)
                {
                    ValidateConfiguration(commandModule);
                }
            }
        }

        #endregion Public Methods

        #region Private Methods

        private void ValidateConfiguration(IActivityMonitorConfiguration config)
        {
            var configClassName = config.GetType().FullName;

            if (string.IsNullOrWhiteSpace(config.MonitorName))
            {
                RaiseValidationEvent(
                    new HyperNodeConfigurationException(
                        $"The MonitorName property is required for {configClassName}."
                    )
                );
            }

            if (string.IsNullOrWhiteSpace(config.MonitorType))
            {
                RaiseValidationEvent(
                    new HyperNodeConfigurationException(
                        $"The MonitorType property is required for {configClassName}."
                    )
                );
            }
            else
            {
                ValidateTypeHasBaseType(config.MonitorType, typeof(HyperNodeServiceActivityMonitor));
            }
        }

        private void ValidateConfiguration(ISystemCommandConfiguration config)
        {
            var configClassName = config.GetType().FullName;

            if (string.IsNullOrWhiteSpace(config.CommandName))
            {
                RaiseValidationEvent(
                    new HyperNodeConfigurationException(
                        $"The CommandName property is required for {configClassName}."
                    )
                );
            }
            else
            {
                if (!SystemCommandName.IsSystemCommand(config.CommandName))
                {
                    RaiseValidationEvent(
                        new HyperNodeConfigurationException(
                            string.Format(
                                "The value '{0}' is not a valid system command name. The following is a list of all recognized system command names:{1}{1}{2}",
                                config.CommandName,
                                Environment.NewLine,
                                string.Join(Environment.NewLine, SystemCommandName.GetAll())
                            )
                        )
                    );
                }
            }
        }

        private void ValidateConfiguration(ICommandModuleConfiguration config)
        {
            var configClassName = config.GetType().FullName;

            if (string.IsNullOrWhiteSpace(config.CommandName))
            {
                RaiseValidationEvent(
                    new HyperNodeConfigurationException(
                        $"The CommandName property is required for {configClassName}."
                    )
                );
            }

            if (string.IsNullOrWhiteSpace(config.CommandModuleType))
            {
                RaiseValidationEvent(
                    new HyperNodeConfigurationException(
                        $"The CommandModuleType property is required for {configClassName}."
                    )
                );
            }
            else
            {
                ValidateTypeImplementsAnyInterface(
                    config.CommandModuleType,
                    typeof(ICommandModule),
                    typeof(IAwaitableCommandModule)
                );
            }

            // RequestSerializerType property is not required, but if it is specified, it must implement the correct interface
            if (!string.IsNullOrWhiteSpace(config.RequestSerializerType))
                ValidateTypeImplementsInterface(config.RequestSerializerType, typeof(ICommandRequestSerializer));

            // ResponseSerializerType property is not required, but if it is specified, it must implement the correct interface
            if (!string.IsNullOrWhiteSpace(config.ResponseSerializerType))
                ValidateTypeImplementsInterface(config.ResponseSerializerType, typeof(ICommandResponseSerializer));
        }

        private void ValidateTypeImplementsInterface(string targetTypeString, Type requiredInterface)
        {
            if (requiredInterface == null)
                throw new ArgumentNullException(nameof(requiredInterface));

            if (!requiredInterface.IsInterface)
                throw new ArgumentException("Type must be an interface.", nameof(requiredInterface));

            ValidateTypeString(targetTypeString, out var targetType);

            if (targetType != null && !targetType.GetInterfaces().Contains(requiredInterface))
            {
                RaiseValidationEvent(
                    new HyperNodeConfigurationException(
                        $"The type '{targetType.FullName}' must implement {requiredInterface.FullName}."
                    )
                );
            }
        }

        private void ValidateTypeImplementsAnyInterface(string targetTypeString, params Type[] acceptedInterfaces)
        {
            if (acceptedInterfaces == null)
                throw new ArgumentNullException(nameof(acceptedInterfaces));

            if (!acceptedInterfaces.All(t => t.IsInterface))
                throw new ArgumentException("Every Type must be an interface.", nameof(acceptedInterfaces));

            ValidateTypeString(targetTypeString, out var targetType);

            if (targetType != null && !targetType.GetInterfaces().Intersect(acceptedInterfaces).Any())
            {
                RaiseValidationEvent(
                    new HyperNodeConfigurationException(
                        $"The type '{targetType.FullName}' must implement one of the following interfaces: {string.Join(", ", acceptedInterfaces.Select(t => t.FullName))}."
                    )
                );
            }
        }

        private void ValidateTypeHasBaseType(string targetTypeString, Type requiredBaseType)
        {
            if (requiredBaseType == null)
                throw new ArgumentNullException(nameof(requiredBaseType));

            if (!requiredBaseType.IsClass)
                throw new ArgumentException("Type must be a class; that is, not a value type or interface.", nameof(requiredBaseType));

            ValidateTypeString(targetTypeString, out var targetType);

            if (targetType != null && !targetType.IsSubclassOf(requiredBaseType))
            {
                RaiseValidationEvent(
                    new HyperNodeConfigurationException(
                        $"The type '{targetType.FullName}' must inherit {requiredBaseType.FullName}."
                    )
                );
            }
        }

        private void ValidateTypeString(string targetTypeString, out Type targetType)
        {
            try
            {
                targetType = Type.GetType(targetTypeString, true);
            }
            catch (Exception ex)
            {
                targetType = null;

                RaiseValidationEvent(
                    new HyperNodeConfigurationException(
                        $"The string '{targetTypeString}' could not be parsed into a {typeof (Type).FullName} object. The string must be an assembly qualified type name. See inner exception for details.",
                        ex
                    )
                );
            }
        }

        private void RaiseValidationEvent(HyperNodeConfigurationException ex)
        {
            _validationEventHandler(
                this,
                new HyperNodeConfigurationValidationEventArgs
                {
                    Exception = ex,
                    Message = ex.Message
                }
            );
        }

        private static void DefaultValidationEventHandler(object sender, HyperNodeConfigurationValidationEventArgs args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(
                    nameof(args),
                    $"An error occurred while validating an instance of {typeof (IHyperNodeConfiguration).FullName}, but no event arguments were supplied."
                );
            }

            throw args.Exception ?? new HyperNodeConfigurationException(
                args.Message ?? $"An error occurred while validating an instance of {typeof (IHyperNodeConfiguration).FullName}, but no error message was specified."
            );
        }

        #endregion Private Methods
    }
}
