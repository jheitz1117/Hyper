namespace Hyper.NodeServices.Contracts.Extensibility
{
    /// <summary>
    /// Marks a class as a request object for a custom command module. Classes implementing this interface can be serialized
    /// and deserialized using an instance of <see cref="ICommandRequestSerializer"/>.
    /// </summary>
    public interface ICommandRequest { }
}
