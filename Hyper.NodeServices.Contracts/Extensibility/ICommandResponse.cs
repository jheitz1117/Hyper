namespace Hyper.NodeServices.Contracts.Extensibility
{
    /// <summary>
    /// Defines a class that is returned from a custom command module. Classes implementing this interface can be serialized
    /// and deserialized using an instance of <see cref="ICommandResponseSerializer"/>.
    /// </summary>
    public interface ICommandResponse
    {
        /// <summary>
        /// A bitwise combination of <see cref="MessageProcessStatusFlags"/> values indicating what happened while the command was running.
        /// </summary>
        MessageProcessStatusFlags ProcessStatusFlags { get; set; }
    }
}
