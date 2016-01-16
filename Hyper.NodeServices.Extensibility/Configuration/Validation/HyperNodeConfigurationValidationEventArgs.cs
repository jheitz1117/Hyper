namespace Hyper.NodeServices.Extensibility.Configuration.Validation
{
    /// <summary>
    /// Returns detailed information related to the <see cref="HyperNodeConfigurationValidationEventHandler"/>.
    /// </summary>
    public class HyperNodeConfigurationValidationEventArgs
    {
        /// <summary>
        /// Gets the <see cref="HyperNodeConfigurationException"/> associated with the validation event.
        /// </summary>
        public HyperNodeConfigurationException Exception { get; set; }

        /// <summary>
        /// Gets the text description corresponding to the validation event.
        /// </summary>
        public string Message { get; set; }
    }
}
