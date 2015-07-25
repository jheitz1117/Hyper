using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Hyper.Services.HyperNodeServices.Configuration
{
    internal sealed class CommandModuleElementCollection : ConfigurationElementCollection, IEnumerable<CommandModuleElement>
    {
        public CommandModuleElement this[int index]
        {
            get { return BaseGet(index) as CommandModuleElement; }
            set
            {
                if (BaseGet(index) != null)
                    BaseRemoveAt(index);
                BaseAdd(index, value);
            }
        }

        public new CommandModuleElement this[string name]
        {
            get { return BaseGet(name) as CommandModuleElement; }
            set
            {
                if (BaseGet(name) != null)
                    BaseRemove(name);
                BaseAdd(value);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new CommandModuleElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((CommandModuleElement)element).Name;
        }

        public new IEnumerator<CommandModuleElement> GetEnumerator()
        {
            return this.OfType<CommandModuleElement>().GetEnumerator();
        }

        [ConfigurationProperty("requestSerializer", IsRequired = false)]
        public string RequestSerializerType
        {
            get { return (string)base["requestSerializer"]; }
            set { base["requestSerializer"] = value; }
        }

        [ConfigurationProperty("responseSerializer", IsRequired = false)]
        public string ResponseSerializerType
        {
            get { return (string)base["responseSerializer"]; }
            set { base["responseSerializer"] = value; }
        }
    }
}
