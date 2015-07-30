using System;
using Hyper.Extensibility.ActivityTracking;

namespace Hyper.NodeServices.Extensibility
{
    public interface IHyperNodeActivityEventItem : IActivityItem
    {
        Guid MessageGuid { get; }
        string CommandName { get; }
        object EventData { get; set; }
        double? ProgressPart { get; set; }
        double? ProgressTotal { get; set; }
        bool IsCompletionEvent { get; }
    }
}
