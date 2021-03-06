﻿using System.Configuration;
using Hyper.NodeServices.Extensibility.Configuration;

namespace Hyper.NodeServices.Configuration
{
    internal sealed class ActivityMonitorElement : ConfigurationElement, IActivityMonitorConfiguration
    {
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string MonitorName
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("type", IsRequired = true)]
        public string MonitorType
        {
            get { return (string)this["type"]; }
            set { this["type"] = value; }
        }

        [ConfigurationProperty("enabled", IsRequired = false)]
        public bool Enabled
        {
            get { return (bool)this["enabled"]; }
            set { this["enabled"] = value; }
        }
    }
}
