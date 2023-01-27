using System;
using System.Collections;
using System.Collections.Generic;
using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Extensibility.CommandModules;

namespace Hyper.NodeServices.Extensibility.Configuration.Model
{
    /// <summary>
    /// Defines configurable properties of a collection of system-level <see cref="ICommandModule"/> objects in an <see cref="IHyperNodeService"/>.
    /// </summary>
    public class SystemCommandConfigurationCollection : ISystemCommandConfigurationCollection
    {
        private readonly Dictionary<string, SystemCommandConfiguration> _systemCommands = new Dictionary<string, SystemCommandConfiguration>();

        /// <summary>
        /// Gets or sets the <see cref="SystemCommandConfiguration"/> with the specified name.
        /// </summary>
        /// <param name="name">The name of the <see cref="SystemCommandConfiguration"/> to get or set.</param>
        /// <returns></returns>
        public SystemCommandConfiguration this[string name]
        {
            get => _systemCommands[name];
            set => _systemCommands[name] = value;
        }

        /// <summary>
        /// Gets the number of <see cref="SystemCommandConfiguration"/> objects contained in the <see cref="SystemCommandConfigurationCollection"/>.
        /// </summary>
        public int Count => _systemCommands.Count;

        /// <summary>
        /// Indicates whether system-level <see cref="ICommandModule"/> objects in the <see cref="IHyperNodeService"/> will be enabled
        /// when the <see cref="IHyperNodeService"/> starts. This property can be overridden for a specific command by adding an
        /// instance of <see cref="ISystemCommandConfiguration"/> to the <see cref="ISystemCommandConfigurationCollection"/> with
        /// the desired configuration.
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<ISystemCommandConfiguration> GetEnumerator()
        {
            return _systemCommands.Values.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Gets the <see cref="ISystemCommandConfiguration"/> object with the specified name.
        /// </summary>
        /// <param name="commandName">The name of the <see cref="ISystemCommandConfiguration"/> object to get.</param>
        /// <returns></returns>
        public ISystemCommandConfiguration GetByCommandName(string commandName)
        {
            if (commandName == null)
                throw new ArgumentNullException(nameof(commandName));

            if (!_systemCommands.ContainsKey(commandName))
                throw new KeyNotFoundException($"No system command exists with the name '{commandName}'.");

            return this[commandName];
        }

        /// <summary>
        /// Determines whether the <see cref="ISystemCommandConfigurationCollection"/> contains an instance of <see cref="ISystemCommandConfiguration"/> with the specified name.
        /// </summary>
        /// <param name="commandName">The name of the <see cref="ISystemCommandConfiguration"/> object to locate in the <see cref="ISystemCommandConfigurationCollection"/>.</param>
        /// <returns></returns>
        public bool ContainsCommandName(string commandName)
        {
            return _systemCommands.ContainsKey(commandName);
        }

        /// <summary>
        /// Adds the specified <see cref="SystemCommandConfiguration"/> object to the <see cref="SystemCommandConfigurationCollection"/>.
        /// </summary>
        /// <param name="config">The <see cref="SystemCommandConfiguration"/> object to add.</param>
        public void Add(SystemCommandConfiguration config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));

            if (string.IsNullOrWhiteSpace(config.CommandName))
            {
                throw new ArgumentException(
                    $"The {nameof(config.CommandName)} property must not be blank.",
                    nameof(config)
                );
            }

            if (_systemCommands.ContainsKey(config.CommandName))
                throw new ArgumentException($"A system command already exists with the name '{config.CommandName}'.");

            _systemCommands.Add(config.CommandName, config);
        }

        /// <summary>
        /// Removes the <see cref="SystemCommandConfiguration"/> object with the specified name from the <see cref="SystemCommandConfigurationCollection"/>.
        /// </summary>
        /// <param name="name">The name of the <see cref="SystemCommandConfiguration"/> object to remove.</param>
        public void Remove(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));

            if (_systemCommands.ContainsKey(name))
                _systemCommands.Remove(name);
        }

        /// <summary>
        /// Removes all <see cref="SystemCommandConfiguration"/> objects from the <see cref="SystemCommandConfigurationCollection"/>.
        /// </summary>
        public void Clear()
        {
            _systemCommands.Clear();
        }
    }
}
