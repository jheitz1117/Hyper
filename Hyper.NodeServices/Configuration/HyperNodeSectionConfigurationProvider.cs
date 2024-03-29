﻿using System.Configuration;
using Hyper.NodeServices.Configuration.ConfigurationSections;
using Hyper.NodeServices.Extensibility;
using Hyper.NodeServices.Extensibility.Configuration;

namespace Hyper.NodeServices.Configuration
{
    internal class HyperNodeSectionConfigurationProvider : IHyperNodeConfigurationProvider
    {
        private const string HyperNodeConfigurationSectionName = "hyperSoa/hyperNode";

        public IHyperNodeConfiguration GetConfiguration()
        {
            var section = (HyperNodeConfigurationSection)ConfigurationManager.GetSection(HyperNodeConfigurationSectionName);
            if (section == null)
                throw new HyperNodeConfigurationException($"The application configuration file does not contain a section with the name '{HyperNodeConfigurationSectionName}'.");

            return section;
        }
    }
}
