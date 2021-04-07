using System.Configuration;
using Hyper.NodeServices.Extensibility.Configuration;

namespace Hyper.NodeServices.Configuration.ConfigurationSections
{
    internal sealed class SystemCommandElement : ConfigurationElement, ISystemCommandConfiguration
    {
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string CommandName
        {
            get => (string)this["name"];
            set => this["name"] = value;
        }

        [ConfigurationProperty("enabled", IsRequired = false)]
        public bool Enabled
        {
            get => (bool)this["enabled"];
            set => this["enabled"] = value;
        }
    }
}
