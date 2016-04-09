using System;
using System.Collections;
using System.Collections.Generic;
using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Extensibility.ActivityTracking;

namespace Hyper.NodeServices.Extensibility.Configuration.Model
{
    /// <summary>
    /// Defines configurable properties of a collection of user-defined <see cref="HyperNodeServiceActivityMonitor"/> objects in an <see cref="IHyperNodeService"/>.
    /// </summary>
    public class ActivityMonitorConfigurationCollection : IActivityMonitorConfigurationCollection
    {
        private readonly Dictionary<string, ActivityMonitorConfiguration> _activityMonitors = new Dictionary<string, ActivityMonitorConfiguration>();

        /// <summary>
        /// Gets or sets the <see cref="ActivityMonitorConfiguration"/> with the specified monitor name.
        /// </summary>
        /// <param name="monitorName">The monitor name of the <see cref="ActivityMonitorConfiguration"/> to get or set.</param>
        /// <returns></returns>
        public ActivityMonitorConfiguration this[string monitorName]
        {
            get { return _activityMonitors[monitorName]; }
            set { _activityMonitors[monitorName] = value; }
        }

        /// <summary>
        /// Gets the number of <see cref="ActivityMonitorConfiguration"/> objects contained in the <see cref="ActivityMonitorConfigurationCollection"/>.
        /// </summary>
        public int Count => _activityMonitors.Count;

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<IActivityMonitorConfiguration> GetEnumerator()
        {
            return _activityMonitors.Values.GetEnumerator();
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
        /// Adds the specified <see cref="ActivityMonitorConfiguration"/> object to the <see cref="ActivityMonitorConfigurationCollection"/>.
        /// </summary>
        /// <param name="config">The <see cref="ActivityMonitorConfiguration"/> object to add.</param>
        public void Add(ActivityMonitorConfiguration config)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));
            
            if (string.IsNullOrWhiteSpace(config.MonitorName))
            {
                throw new ArgumentException(
                    $"The {nameof(config.MonitorName)}. property must not be blank.",
                    nameof(config)
                );
            }

            if (_activityMonitors.ContainsKey(config.MonitorName))
                throw new ArgumentException($"An activity monitor already exists with the monitor name '{config.MonitorName}'.");

            _activityMonitors.Add(config.MonitorName, config);
        }

        /// <summary>
        /// Removes the <see cref="ActivityMonitorConfiguration"/> object with the specified monitor name from the <see cref="ActivityMonitorConfigurationCollection"/>.
        /// </summary>
        /// <param name="monitorName">The monitor name of the <see cref="ActivityMonitorConfiguration"/> object to remove.</param>
        public void Remove(string monitorName)
        {
            if (monitorName == null)
                throw new ArgumentNullException(nameof(monitorName));
            
            if (_activityMonitors.ContainsKey(monitorName))
                _activityMonitors.Remove(monitorName);
        }

        /// <summary>
        /// Removes all <see cref="ActivityMonitorConfiguration"/> objects from the <see cref="ActivityMonitorConfigurationCollection"/>.
        /// </summary>
        public void Clear()
        {
            _activityMonitors.Clear();
        }
    }
}
