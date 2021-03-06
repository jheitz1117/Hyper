﻿using System;
using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Extensibility.ActivityTracking;

namespace Hyper.NodeServices.ActivityTracking
{
    internal sealed class ChildNodeResponseMonitor : HyperNodeServiceActivityMonitor
    {
        private readonly HyperNodeMessageResponse _target;
        
        public ChildNodeResponseMonitor(HyperNodeMessageResponse target)
        {
            Name = nameof(ChildNodeResponseMonitor);
            _target = target;
        }

        /// <summary>
        /// Filter out events that don't contain a response from a child node.
        /// </summary>
        /// <param name="activity"><see cref="HyperNodeActivityEventItem"/> object to use to filter.</param>
        /// <returns></returns>
        public override bool ShouldTrack(IHyperNodeActivityEventItem activity)
        {
            // Not only do we want to make sure the event data object is an instance of HyperNodeMessageResponse,
            // but we also want to make sure it came from a child node instead of this node. To do this, we can
            // check the RespondingNodeName of the response and see if the response came from a node other than
            // this one.
            var response = activity.EventData as HyperNodeMessageResponse;
            return response != null && response.RespondingNodeName != activity.Agent;
        }

        public override void OnTrack(IHyperNodeActivityEventItem activity)
        {
            // Check if we've received a HyperNodeMessageResponse in the data property.
            var response = activity.EventData as HyperNodeMessageResponse;
            if (response != null)
                _target.ChildResponses.TryAdd(response.RespondingNodeName, response);
        }

        public override void OnActivityReportingError(Exception exception)
        {
            // There's nothing reasonable we can do here. Just eat the exception and let another observer record the error.
        }
    }
}
