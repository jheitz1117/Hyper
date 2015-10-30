using System;
using System.Collections;
using System.Collections.Generic;

namespace Hyper.NodeServices.Extensibility.Configuration.Model
{
    // TODO: XDOC this class
    public class SystemCommandConfigurationCollection : ISystemCommandConfigurationCollection
    {
        private readonly Dictionary<string, SystemCommandConfiguration> _systemCommands = new Dictionary<string, SystemCommandConfiguration>();

        public SystemCommandConfiguration this[string name]
        {
            get { return _systemCommands[name]; }
            set { _systemCommands[name] = value; }
        }

        public int Count
        {
            get { return _systemCommands.Count; }
        }

        public bool Enabled { get; set; }

        public IEnumerator<ISystemCommandConfiguration> GetEnumerator()
        {
            return _systemCommands.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(SystemCommandConfiguration config)
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

            if (_systemCommands.ContainsKey(config.Name))
                throw new ArgumentException("A system command already exists with the name '" + config.Name + "'.");

            _systemCommands.Add(config.Name, config);
        }

        public void Remove(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            if (_systemCommands.ContainsKey(name))
                _systemCommands.Remove(name);
        }

        public void Clear()
        {
            _systemCommands.Clear();
        }

        public ISystemCommandConfiguration GetByCommandName(string commandName)
        {
            if (commandName == null)
                throw new ArgumentNullException("commandName");
            
            if (!_systemCommands.ContainsKey(commandName))
                throw new KeyNotFoundException("No system command exists with the name '" + commandName + "'.");
            
            return this[commandName];
        }
    }
}
