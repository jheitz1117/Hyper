using System;
using Hyper.Extensibility.ActivityTracking;
using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Extensibility.CommandModules;

namespace Hyper.NodeServices.Extensibility.ActivityTracking
{
    /// <summary>
    /// Describes an <see cref="IActivityItem"/> event raised by a <see cref="IHyperNodeService"/>.
    /// </summary>
    public interface IHyperNodeActivityEventItem : IActivityItem
    {
        /// <summary>
        /// The ID of the current task.
        /// </summary>
        string TaskId { get; }

        /// <summary>
        /// The name of the command currently executing.
        /// </summary>
        string CommandName { get; }

        /// <summary>
        /// The amount of time that has elapsed since the first <see cref="IHyperNodeActivityEventItem"/> was raised for this task.
        /// This value may be null unless diagnostics are enabled.
        /// </summary>
        TimeSpan? Elapsed { get; }

        /// <summary>
        /// An object representing data associated with this <see cref="IHyperNodeActivityEventItem"/>. This value may be null.
        /// </summary>
        object EventData { get; set; }

        /// <summary>
        /// A numeric value representing the progress of a <see cref="ICommandModule"/>. This value may be used in conjunction
        /// with the <see cref="ProgressTotal"/> property to obtain a percentile.
        /// </summary>
        double? ProgressPart { get; set; }

        /// <summary>
        /// A numeric value representing the progress total of a <see cref="ICommandModule"/>. This value may be used in conjunction
        /// with the <see cref="ProgressPart"/> property to obtain a percentile.
        /// </summary>
        double? ProgressTotal { get; set; }
    }
}
