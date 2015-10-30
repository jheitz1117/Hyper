using System;
using System.Collections;
using System.Collections.Generic;

namespace Hyper.NodeServices.Extensibility.Configuration.Model
{
    // TODO: XDOC this class
    public class CommandModuleConfigurationCollection : ICommandModuleConfigurationCollection
    {
        private readonly Dictionary<string, CommandModuleConfiguration> _commandModules = new Dictionary<string, CommandModuleConfiguration>();

        public CommandModuleConfiguration this[string name]
        {
            get { return _commandModules[name]; }
            set { _commandModules[name] = value; }
        }

        public int Count
        {
            get { return _commandModules.Count; }
        }

        public string RequestSerializerType { get; set; }
        public string ResponseSerializerType { get; set; }

        public IEnumerator<ICommandModuleConfiguration> GetEnumerator()
        {
            return _commandModules.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(CommandModuleConfiguration config)
        {
            if (config == null)
                throw new ArgumentNullException("config");

            if (string.IsNullOrWhiteSpace(config.Name))
            {
                throw new ArgumentException(
                    string.Format(
                        "The {0}.Name property must not be blank.",
                        config.GetType().FullName
                    ),
                    "config"
                );
            }

            if (_commandModules.ContainsKey(config.Name))
                throw new ArgumentException("A command module already exists with the name '" + config.Name + "'.");

            _commandModules.Add(config.Name, config);
        }

        public void Remove(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            if (_commandModules.ContainsKey(name))
                _commandModules.Remove(name);
        }

        public void Clear()
        {
            _commandModules.Clear();
        }
    }
}
