using System;
using System.Collections;
using System.Collections.Generic;
using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Contracts.Extensibility.CommandModules;
using Hyper.NodeServices.Extensibility.CommandModules;

namespace Hyper.NodeServices.Extensibility.Configuration.Model
{
    /// <summary>
    /// Defines configurable properties of a collection of user-defined <see cref="ICommandModule"/> objects in an <see cref="IHyperNodeService"/>.
    /// </summary>
    public class CommandModuleConfigurationCollection : ICommandModuleConfigurationCollection
    {
        private readonly Dictionary<string, CommandModuleConfiguration> _commandModules = new Dictionary<string, CommandModuleConfiguration>();

        /// <summary>
        /// Gets or sets the <see cref="CommandModuleConfiguration"/> with the specified command name.
        /// </summary>
        /// <param name="commandName">The command name of the <see cref="CommandModuleConfiguration"/> to get or set.</param>
        /// <returns></returns>
        public CommandModuleConfiguration this[string commandName]
        {
            get => _commandModules[commandName];
            set => _commandModules[commandName] = value;
        }

        /// <summary>
        /// Gets the number of <see cref="CommandModuleConfiguration"/> objects contained in the <see cref="CommandModuleConfigurationCollection"/>.
        /// </summary>
        public int Count => _commandModules.Count;

        /// <summary>
        /// The assembly qualified name of a type that implements <see cref="ICommandRequestSerializer"/>. If this property is supplied,
        /// the specified type serves as the default <see cref="ICommandRequestSerializer"/> implementation for all user-defined
        /// <see cref="ICommandModule"/> objects in the <see cref="ICommandModuleConfigurationCollection"/>. This collection-level
        /// default can be overridden for a specific command by supplying the desired <see cref="ICommandRequestSerializer"/> type
        /// string in the <see cref="ICommandModuleConfiguration.RequestSerializerType"/> property for the <see cref="ICommandModuleConfiguration"/>
        /// for that command.
        /// </summary>
        public string RequestSerializerType { get; set; }

        /// <summary>
        /// The assembly qualified name of a type that implements <see cref="ICommandResponseSerializer"/>. If this property is supplied,
        /// the specified type serves as the default <see cref="ICommandResponseSerializer"/> implementation for all user-defined
        /// <see cref="ICommandModule"/> objects in the <see cref="ICommandModuleConfigurationCollection"/>. This collection-level
        /// default can be overridden for a specific command by supplying the desired <see cref="ICommandResponseSerializer"/> type
        /// string in the <see cref="ICommandModuleConfiguration.ResponseSerializerType"/> property for the <see cref="ICommandModuleConfiguration"/>
        /// for that command.
        /// </summary>
        public string ResponseSerializerType { get; set; }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<ICommandModuleConfiguration> GetEnumerator()
        {
            return _commandModules.Values.GetEnumerator();
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
        /// Adds the specified <see cref="CommandModuleConfiguration"/> object to the <see cref="CommandModuleConfigurationCollection"/>.
        /// </summary>
        /// <param name="config">The <see cref="CommandModuleConfiguration"/> object to add.</param>
        public void Add(CommandModuleConfiguration config)
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

            if (_commandModules.ContainsKey(config.CommandName))
                throw new ArgumentException($"A command module already exists with the command name '{config.CommandName}'.");

            _commandModules.Add(config.CommandName, config);
        }

        /// <summary>
        /// Removes the <see cref="CommandModuleConfiguration"/> object with the specified command name from the <see cref="CommandModuleConfigurationCollection"/>.
        /// </summary>
        /// <param name="commandName">The command name of the <see cref="CommandModuleConfiguration"/> object to remove.</param>
        public void Remove(string commandName)
        {
            if (commandName == null)
                throw new ArgumentNullException(nameof(commandName));

            if (_commandModules.ContainsKey(commandName))
                _commandModules.Remove(commandName);
        }

        /// <summary>
        /// Removes all <see cref="CommandModuleConfiguration"/> objects from the <see cref="CommandModuleConfigurationCollection"/>.
        /// </summary>
        public void Clear()
        {
            _commandModules.Clear();
        }
    }
}
