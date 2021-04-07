using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Hyper.NodeServices.Extensibility.Configuration;

namespace Hyper.NodeServices.Configuration.ConfigurationSections
{
    internal sealed class SystemCommandElementCollection : ConfigurationElementCollection, ISystemCommandConfigurationCollection
    {
        public SystemCommandElement this[int index]
        {
            get => BaseGet(index) as SystemCommandElement;
            set
            {
                if (BaseGet(index) != null)
                    BaseRemoveAt(index);
                BaseAdd(index, value);
            }
        }

        public new SystemCommandElement this[string name]
        {
            get => BaseGet(name) as SystemCommandElement;
            set
            {
                if (BaseGet(name) != null)
                    BaseRemove(name);
                BaseAdd(value);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new SystemCommandElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((SystemCommandElement)element).CommandName;
        }

        public new IEnumerator<ISystemCommandConfiguration> GetEnumerator()
        {
            return this.OfType<SystemCommandElement>().GetEnumerator();
        }

        [ConfigurationProperty("enabled", IsRequired = false)]
        public bool Enabled
        {
            get => (bool)base["enabled"];
            set => base["enabled"] = value;
        }

        public ISystemCommandConfiguration GetByCommandName(string commandName)
        {
            return this[commandName];
        }

        public bool ContainsCommandName(string commandName)
        {
            return BaseGetAllKeys().Any(k => k.ToString() == commandName);
        }
    }
}
