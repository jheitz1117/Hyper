using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Hyper.NodeServices.Configuration
{
    internal sealed class SystemCommandElementCollection : ConfigurationElementCollection, IEnumerable<SystemCommandElement>
    {
        public SystemCommandElement this[int index]
        {
            get { return BaseGet(index) as SystemCommandElement; }
            set
            {
                if (BaseGet(index) != null)
                    BaseRemoveAt(index);
                BaseAdd(index, value);
            }
        }

        public new SystemCommandElement this[string name]
        {
            get { return BaseGet(name) as SystemCommandElement; }
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
            return ((SystemCommandElement)element).Name;
        }

        public new IEnumerator<SystemCommandElement> GetEnumerator()
        {
            return this.OfType<SystemCommandElement>().GetEnumerator();
        }

        [ConfigurationProperty("enabled", IsRequired = false, DefaultValue = true)]
        public bool Enabled
        {
            get { return (bool)base["enabled"]; }
            set { base["enabled"] = value; }
        }
    }
}
