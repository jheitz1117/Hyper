using Hyper.NodeServices.Contracts;

namespace Hyper.NodeServices.Extensibility.EventTracking
{
    /// <summary>
    /// Event arguments passed to an implementation of <see cref="IHyperNodeEventHandler"/> when the <see cref="IHyperNodeService"/> chooses to ignore a message.
    /// </summary>
    public interface IMessageIgnoredEventArgs : IHyperNodeEventArgs
    {
        /// <summary>
        /// The reason why the message was ignored.
        /// </summary>
        string Reason { get; }
    }
}
