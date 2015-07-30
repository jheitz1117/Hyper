using System;
using Hyper.NodeServices.Extensibility;

namespace Hyper.NodeServices.ActivityTracking
{
    internal sealed class HyperNodeActivityEventItem : IHyperNodeActivityEventItem
    {
        // These properties have an internal setter because they are hard-set by the activity tracker
        public Guid MessageGuid { get; internal set; }
        public string CommandName { get; internal set; }

        // These properties are free to be modified by the user if they wish
        public DateTime EventDateTime { get; set; }
        public string Agent { get; set; }
        public string EventDescription { get; set; }
        public string EventDetail { get; set; }
        public object EventData { get; set; }
        public double? ProgressPart { get; set; }
        public double? ProgressTotal { get; set; }

        private readonly bool _isCompletionEvent;
        public bool IsCompletionEvent { get { return _isCompletionEvent; } }

        internal HyperNodeActivityEventItem()
        {
            this.EventDateTime = DateTime.Now;
        }

        internal HyperNodeActivityEventItem(bool isCompletionEvent)
            : this()
        {
            _isCompletionEvent = isCompletionEvent;
        }
    }
}
