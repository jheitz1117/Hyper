using System;
using Hyper.Extensibility.ActivityTracking;

namespace Hyper.NodeServices.Extensibility.ActivityTracking
{
    public interface IHyperNodeActivityEventItem : IActivityItem
    {
        Guid MessageGuid { get; }
        string TaskId { get; }
        string CommandName { get; }
        TimeSpan? Elapsed { get; }
        object EventData { get; set; }
        double? ProgressPart { get; set; }
        double? ProgressTotal { get; set; }
    }
}
