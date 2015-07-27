using System;
using Hyper.Extensibility.ActivityTracking;

namespace Hyper.Services.HyperNodeExtensibility
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
