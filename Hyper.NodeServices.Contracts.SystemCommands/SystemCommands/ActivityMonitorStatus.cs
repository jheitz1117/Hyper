using System.Runtime.Serialization;

namespace Hyper.NodeServices.Contracts.SystemCommands
{
    /// <summary>
    /// Describes the status of a custom activity monitor.
    /// </summary>
    [DataContract]
    public class ActivityMonitorStatus
    {
        /// <summary>
        /// The name of the custom activity monitor.
        /// </summary>
        [DataMember]
        public string Name { get; set; }
        
        /// <summary>
        /// Indicates whether the custom activity monitor is enabled or disabled.
        /// </summary>
        [DataMember]
        public bool Enabled { get; set; }
    }
}
