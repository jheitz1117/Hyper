using System.Configuration;
using Hyper.NodeServices.Extensibility.Configuration;

namespace Hyper.NodeServices.Configuration.ConfigurationSection
{
    internal sealed class CommandModuleElement : ConfigurationElement, ICommandModuleConfiguration
    {
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string CommandName
        {
            get => (string)this["name"];
            set => this["name"] = value;
        }

        [ConfigurationProperty("type", IsRequired = true)]
        public string CommandModuleType
        {
            get => (string)this["type"];
            set => this["type"] = value;
        }

        [ConfigurationProperty("enabled", IsRequired = false)]
        public bool Enabled
        {
            get => (bool)this["enabled"];
            set => this["enabled"] = value;
        }

        [ConfigurationProperty("requestSerializer", IsRequired = false)]
        public string RequestSerializerType
        {
            get => (string)this["requestSerializer"];
            set => this["requestSerializer"] = value;
        }

        [ConfigurationProperty("responseSerializer", IsRequired = false)]
        public string ResponseSerializerType
        {
            get => (string)this["responseSerializer"];
            set => this["responseSerializer"] = value;
        }
    }
}
