using System;
using Hyper.Services.HyperNodeExtensibility;

namespace Hyper.Services.HyperNodeServices.CommandModules
{
    internal class CommandModuleConfiguration
    {
        public string CommandName { get; set; }
        public bool Enabled { get; set; }
        public Type CommandModuleType { get; set; }
        public ICommandModuleRequestSerializer RequestSerializer { get; set; }
        public ICommandModuleResponseSerializer ResponseSerializer { get; set; }
    }
}
