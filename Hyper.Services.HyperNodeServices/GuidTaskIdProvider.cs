using System;
using Hyper.NodeServices.Contracts;
using Hyper.Services.HyperNodeExtensibility;

namespace Hyper.Services.HyperNodeServices
{
    internal sealed class GuidTaskIdProvider : ITaskIdProvider
    {
        public string CreateTaskId(HyperNodeMessageRequest message)
        {
            return Guid.NewGuid().ToString();
        }
    }
}
