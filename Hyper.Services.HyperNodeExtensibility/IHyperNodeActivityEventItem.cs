using System;

namespace Hyper.Services.HyperNodeExtensibility
{
    public interface IHyperNodeActivityEventItem
    {
        Guid MessageGuid { get; }
        string CommandName { get; }
        DateTime EventDateTime { get; set; }
        string Agent { get; set; }
        string EventDescription { get; set; }
        string EventDetail { get; set; }
        object EventData { get; set; }
        double? ProgressPart { get; set; }
        double? ProgressTotal { get; set; }
        bool IsCompletionEvent { get; }
    }
}
