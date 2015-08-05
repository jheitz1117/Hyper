using System;

namespace Hyper.NodeServices.ActivityTracking
{
    internal class HyperNodeActivityContext
    {
        private readonly string _hyperNodeName;
        private readonly Guid _messageGuid;
        private readonly string _commandName;
        private readonly string _taskId;

        public string HyperNodeName { get { return _hyperNodeName; } }
        public Guid MessageGuid { get { return _messageGuid; } }
        public string CommandName { get { return _commandName; } }
        public string TaskId { get { return _taskId; } }

        public HyperNodeActivityContext(string hyperNodeName, Guid messageGuid, string commandName, string taskId)
        {
            _hyperNodeName = hyperNodeName;
            _messageGuid = messageGuid;
            _commandName = commandName;
            _taskId = taskId;
        }
    }
}
