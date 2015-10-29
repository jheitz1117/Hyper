using System;
using Hyper.NodeServices.Extensibility.Configuration;

namespace Hyper.NodeServices.Configuration.Validation
{
    // Keep in mind now that we're going to have two levels of validation (XSD and manual validation), we'll have to keep them in sync.
    // The reason we can't simplify this is because of intellisense: We can't elliminate XSD because then we lose intellisense in the
    // app.config. However, we can't eliminate validation in this provider class because we would lose support for the default values.

    // To clarify about the default values, the default values used to be supplied by the configuration element classes that integrate
    // with the app.config custom section. However, now that we're supporting multiple configuration strategies, we had to move the
    // default values to a centralized location, i.e., this provider class.
    
    // Finally, since we're doing validation here, we must check required values in addition to supplying default values since there's
    // no guarantee the XSD was ever used to validate the input. We don't even know if the input was in XML format to begin with.
    internal class HyperNodeConfigurationValidator
    {
        private readonly ValidationEventHandler _validationEventHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="HyperNodeConfigurationValidator"/> class.
        /// </summary>
        /// <param name="validationEventHandler">Callback for validation errors. If this is null, an exception is thrown if any validation errors occur.</param>
        public HyperNodeConfigurationValidator(ValidationEventHandler validationEventHandler)
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
            { throw new ArgumentNullException("config"); }

            // TODO: Determine if we should expose the configuration validator as a tool in the extensibility library. This would come in very handy for unit testing...

            // TODO: Check required configuration values. On error, call RaiseValidationEvent().
            if (string.IsNullOrWhiteSpace(config.HyperNodeName))
            {
                RaiseValidationEvent(
                    new HyperNodeConfigurationException(
                        string.Format(
                            "Each instance of {0} must have a unique name.",
                            typeof(HyperNodeService).FullName
                        )
                    )
                );
            }
        }

        private void RaiseValidationEvent(HyperNodeConfigurationException ex)
        {
            _validationEventHandler(
                this,
                new ValidationEventArgs
                {
                    Exception = ex,
                    Message = ex.Message
                }
            );
        }

        private static void DefaultValidationEventHandler(object sender, ValidationEventArgs args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(
                    "args",
                    string.Format(
                        "An error occurred while validating an instance of {0}, but no event arguments were supplied.",
                        typeof (IHyperNodeConfiguration).FullName
                    )
                );
            }

            throw args.Exception ?? new HyperNodeConfigurationException(
                args.Message ?? string.Format(
                    "An error occurred while validating an instance of {0}, but no error message was specified.",
                    typeof(IHyperNodeConfiguration).FullName
                )
            );
        }
    }

    /// <summary>
    /// Represents the callback method that will handle configuration validation events and the <see cref="ValidationEventArgs"/>.
    /// </summary>
    internal delegate void ValidationEventHandler(object sender, ValidationEventArgs e);
}
