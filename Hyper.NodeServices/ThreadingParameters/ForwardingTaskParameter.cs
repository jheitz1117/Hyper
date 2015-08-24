namespace Hyper.NodeServices
{
    internal class ForwardingTaskParameter
    {
        public string ChildNodeName { get; private set; }
        public HyperNodeTaskInfo TaskInfo { get; private set; }

        public ForwardingTaskParameter(string childNodeName, HyperNodeTaskInfo taskInfo)
        {
            this.ChildNodeName = childNodeName;
            this.TaskInfo = taskInfo;
        }
    }
}
