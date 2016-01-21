namespace Hyper.NodeServices
{
    internal class ForwardingTaskParameter
    {
        public string RemoteNodeName { get; private set; }
        public HyperNodeTaskInfo TaskInfo { get; private set; }

        public ForwardingTaskParameter(string remoteNodeName, HyperNodeTaskInfo taskInfo)
        {
            this.RemoteNodeName = remoteNodeName;
            this.TaskInfo = taskInfo;
        }
    }
}
