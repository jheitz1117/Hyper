using Hyper.NodeServices.Contracts;

namespace Hyper.NodeServices.Extensibility.EventTracking
{
    /// <summary>
    /// Event arguments passed to an implementation of <see cref="IHyperNodeEventHandler"/> when the <see cref="IHyperNodeService"/> receives a <see cref="HyperNodeMessageResponse"/>
    /// from another <see cref="IHyperNodeService"/> to which the current message had been previously forwarded.
    /// This event only fires when the remote call was successfully executed and did not timeout or throw any faults or other exceptions.
    /// </summary>
    public interface IHyperNodeRespondedEventArgs : IHyperNodeEventArgs
    {
        /// <summary>
        /// The name of the <see cref="IHyperNodeService"/> from which the response was received.
        /// </summary>
        string RespondingNodeName { get; }

        /// <summary>
        /// The response received from the remote <see cref="IHyperNodeService"/>.
        /// </summary>
        IReadOnlyHyperNodeResponseInfo ResponseInfo { get; }
    }
}
