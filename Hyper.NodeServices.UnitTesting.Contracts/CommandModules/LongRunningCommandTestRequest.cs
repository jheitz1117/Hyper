using System;
using System.Runtime.Serialization;
using Hyper.NodeServices.Contracts.Extensibility;

namespace Hyper.NodeServices.UnitTesting.Contracts.CommandModules
{
    [DataContract]
    public class LongRunningCommandTestRequest : ICommandRequest
    {
        [DataMember]
        public TimeSpan? TotalRunTime { get; set; }

        [DataMember]
        public TimeSpan? MinimumSleepInterval { get; set; }

        [DataMember]
        public TimeSpan? MaximumSleepInterval { get; set; }
    }
}
