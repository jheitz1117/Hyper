using System.Configuration;
using Hyper.NodeServices.Extensibility.Configuration;

namespace Hyper.NodeServices.Configuration
{
    internal sealed class CommandModuleElement : ConfigurationElement, ICommandModuleConfiguration
    {
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string CommandName
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("type", IsRequired = true)]
        public string CommandModuleType
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

        [ConfigurationProperty("requestSerializer", IsRequired = false)]
        public string RequestSerializerType
        {
            get { return (string)this["requestSerializer"]; }
            set { this["requestSerializer"] = value; }
        }

        [ConfigurationProperty("responseSerializer", IsRequired = false)]
        public string ResponseSerializerType
        {
            get { return (string)this["responseSerializer"]; }
            set { this["responseSerializer"] = value; }
        }
    }
}
