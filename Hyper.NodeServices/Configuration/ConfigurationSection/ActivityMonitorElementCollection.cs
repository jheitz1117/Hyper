using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Hyper.NodeServices.Extensibility.Configuration;

namespace Hyper.NodeServices.Configuration
{
    internal sealed class ActivityMonitorElementCollection : ConfigurationElementCollection, IActivityMonitorConfigurationCollection
    {
        public ActivityMonitorElement this[int index]
        {
            get { return BaseGet(index) as ActivityMonitorElement; }
            set
            {
                if (BaseGet(index) != null)
                    BaseRemoveAt(index);
                BaseAdd(index, value);
            }
        }

        public new ActivityMonitorElement this[string name]
        {
            get { return BaseGet(name) as ActivityMonitorElement; }
            set
            {
                if (BaseGet(name) != null)
                    BaseRemove(name);
                BaseAdd(value);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new ActivityMonitorElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ActivityMonitorElement)element).Name;
        }

        public new IEnumerator<IActivityMonitorConfiguration> GetEnumerator()
        {
            return this.OfType<ActivityMonitorElement>().GetEnumerator();
        }
    }
}
