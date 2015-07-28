using Hyper.Services.HyperNodeContracts;
using Hyper.Services.HyperNodeExtensibility;

namespace Hyper.Services.HyperNodeServices
{
    internal sealed class HyperNodeMessageResponseCollector : HyperNodeServiceActivityMonitor
    {
        private readonly HyperNodeMessageResponse _target;
        
        public HyperNodeMessageResponseCollector(HyperNodeMessageResponse target)
        {
            this.Name = GetType().Name;
            _target = target;
        }

        /// <summary>
        /// Filter out events that don't contain a response from a child node.
        /// </summary>
        /// <param name="activity"><see cref="HyperNodeActivityEventItem"/> object to use to filter.</param>
        /// <returns></returns>
        public override bool ShouldTrack(IHyperNodeActivityEventItem activity)
        {
            return activity.EventData is HyperNodeMessageResponse;
        }

        public override void OnNext(IHyperNodeActivityEventItem activity)
        {
            // Check if we've received a HyperNodeMessageResponse in the data property.
            var response = activity.EventData as HyperNodeMessageResponse;
            if (response != null)
                _target.ChildResponses.TryAdd(response.RespondingNodeName, response);
        }
    }
}
