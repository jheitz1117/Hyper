using System;
using System.Runtime.Serialization;
using Hyper.NodeServices.Contracts.Extensibility.CommandModules;

namespace Hyper.NodeServices.UnitTesting.Contracts.CommandModules
{
    /// <summary>
    /// Specifies parameters for a long-running command module.
    /// </summary>
    [DataContract]
    public class LongRunningCommandTestRequest : ICommandRequest
    {
        /// <summary>
        /// Specifies the maximum run time for the command. If this value is null, a default value is chosen by the command module.
        /// </summary>
        [DataMember]
        public TimeSpan? TotalRunTime { get; set; }

        /// <summary>
        /// Specifies the minimum amount of time to wait between progress reports. If this value is null, a default minimum is chosen by the command module.
        /// </summary>
        [DataMember]
        public TimeSpan? MinimumSleepInterval { get; set; }

        /// <summary>
        /// Specifies the maximum amount of time to wait between progress reports. If this value is null, a default maximum is chosen by the command module.
        /// </summary>
        [DataMember]
        public TimeSpan? MaximumSleepInterval { get; set; }
    }
}
