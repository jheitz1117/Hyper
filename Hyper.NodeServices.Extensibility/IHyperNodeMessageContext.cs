using System;
using System.Collections.Generic;
using Hyper.NodeServices.Contracts;

namespace Hyper.NodeServices.Extensibility
{
    /// <summary>
    /// Contains information from <see cref="HyperNodeMessageRequest"/> objects being processed by <see cref="IHyperNodeService"/> instances.
    /// </summary>
    public interface IHyperNodeMessageContext
    {
        /// <summary>
        /// The name of the command specified in the <see cref="HyperNodeMessageRequest"/>.
        /// </summary>
        string CommandName { get; }

        /// <summary>
        /// The name of the agent that created the <see cref="HyperNodeMessageRequest"/>.
        /// </summary>
        string CreatedByAgentName { get; }

        /// <summary>
        /// The date and time at which the <see cref="HyperNodeMessageRequest"/> was created.
        /// </summary>
        DateTime CreationDateTime { get; }

        /// <summary>
        /// The list of <see cref="IHyperNodeService"/> instances to which the <see cref="HyperNodeMessageRequest"/> was intended to be sent.
        /// </summary>
        IReadOnlyList<string> IntendedRecipientNodeNames { get; }

        /// <summary>
        /// The list of <see cref="IHyperNodeService"/> instances that have already seen the <see cref="HyperNodeMessageRequest"/>.
        /// </summary>
        IReadOnlyList<string> SeenByNodeNames { get; }

        /// <summary>
        /// The processing flags specified in the <see cref="IHyperNodeService"/>.
        /// </summary>
        MessageProcessOptionFlags ProcessOptionFlags { get; }
    }
}
