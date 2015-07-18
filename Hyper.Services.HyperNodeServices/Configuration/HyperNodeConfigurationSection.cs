using System.Configuration;

namespace Hyper.Services.HyperNodeServices.Configuration
{
    public sealed class HyperNodeConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("name", DefaultValue = "HyperNode1", IsRequired = true)]
        public string HyperNodeName
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("enableActivityCache", DefaultValue = false, IsRequired = false)]
        public bool EnableActivityCache
        {
            get { return (bool)this["enableActivityCache"]; }
            set { this["enableActivityCache"] = value; }
        }

        [ConfigurationProperty("activityCacheSlidingExpiration", DefaultValue = 60, IsRequired = false)]
        public int ActivityCacheSlidingExpirationMinutes
        {
            get { return (int)this["activityCacheSlidingExpiration"]; }
            set { this["activityCacheSlidingExpiration"] = value; }
        }

        [ConfigurationProperty("activityMonitors")]
        [ConfigurationCollection(typeof(ActivityMonitorElementCollection))]
        public ActivityMonitorElementCollection ActivityMonitors
        {
            get { return this["activityMonitors"] as ActivityMonitorElementCollection; }
        }
    }
}
