using System;
using Hyper.NodeServices.Contracts.Extensibility;

namespace Hyper.Services.HyperNodeServices.CommandModules
{
    internal class CommandModuleConfiguration
    {
        public string CommandName { get; set; }
        public bool Enabled { get; set; }
        public Type CommandModuleType { get; set; }
        public ICommandRequestSerializer RequestSerializer { get; set; }
        public ICommandResponseSerializer ResponseSerializer { get; set; }
    }
}
