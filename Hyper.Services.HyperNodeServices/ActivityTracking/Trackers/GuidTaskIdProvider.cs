using System;
using Hyper.Services.HyperNodeContracts;
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
