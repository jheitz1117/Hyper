using System;
using System.Collections;
using System.Collections.Generic;

namespace Hyper.NodeServices.Extensibility.Configuration.Model
{
    // TODO: XDOC this class
    public class ActivityMonitorConfigurationCollection : IActivityMonitorConfigurationCollection
    {
        private readonly Dictionary<string, ActivityMonitorConfiguration> _activityMonitors = new Dictionary<string, ActivityMonitorConfiguration>();

        public ActivityMonitorConfiguration this[string name]
        {
            get { return _activityMonitors[name]; }
            set { _activityMonitors[name] = value; }
        }

        public int Count
        {
            get { return _activityMonitors.Count; }
        }

        public IEnumerator<IActivityMonitorConfiguration> GetEnumerator()
        {
            return _activityMonitors.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        
        public void Add(ActivityMonitorConfiguration config)
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

            if (_activityMonitors.ContainsKey(config.Name))
                throw new ArgumentException("An activity monitor already exists with the name '" + config.Name + "'.");

            _activityMonitors.Add(config.Name, config);
        }

        public void Remove(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            
            if (_activityMonitors.ContainsKey(name))
                _activityMonitors.Remove(name);
        }

        public void Clear()
        {
            _activityMonitors.Clear();
        }
    }
}
