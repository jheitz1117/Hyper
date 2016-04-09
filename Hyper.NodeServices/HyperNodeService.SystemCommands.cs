using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel.Configuration;
using Hyper.NodeServices.CommandModules;
using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.SystemCommands.Contracts;

namespace Hyper.NodeServices
{
    public partial class HyperNodeService
    {
        #region System Commands

        internal HyperNodeTaskProgressInfo GetCachedTaskProgressInfo(string taskId)
        {
            return _taskProgressCacheMonitor.GetTaskProgressInfo(taskId);
        }

        internal IEnumerable<CommandStatus> GetCommandStatuses()
        {
            return _commandModuleConfigurations.Keys.Select(
                commandName => new CommandStatus
                {
                    CommandName = commandName,
                    CommandType = (SystemCommandName.IsSystemCommand(commandName) ? HyperNodeCommandType.SystemCommand : HyperNodeCommandType.CustomCommand),
                    Enabled = _commandModuleConfigurations[commandName].Enabled
                }
            );
        }

        internal IEnumerable<ActivityMonitorStatus> GetActivityMonitorStatuses()
        {
            lock (Lock)
            {
                return _customActivityMonitors.Select(
                    m => new ActivityMonitorStatus
                    {
                        Name = m.Name,
                        Enabled = m.Enabled
                    }
                );
            }
        }

        internal IEnumerable<LiveTaskStatus> GetLiveTaskStatuses()
        {
            return _liveTasks.Keys.Select(
                taskId => new LiveTaskStatus
                {
                    TaskId = taskId,
                    CommandName = _liveTasks[taskId].Message.CommandName,
                    IsCancellationRequested = _liveTasks[taskId].Token.IsCancellationRequested,
                    Elapsed = _liveTasks[taskId].Elapsed
                }
            );
        }

        internal IEnumerable<string> GetChildNodes()
        {
            var childNodes = new List<string>();

            // Check the app.config for client endpoints for the IHyperNodeService interface
            var configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var serviceModelGroup = ServiceModelSectionGroup.GetSectionGroup(configuration);
            if (serviceModelGroup != null)
            {
                childNodes.AddRange(
                    serviceModelGroup.Client.Endpoints
                        .Cast<ChannelEndpointElement>()
                        .Where(e => e.Contract == typeof(IHyperNodeService).FullName)
                        .Select(e => e.Name)
                );
            }

            return childNodes;
        }

        internal bool IsKnownCommand(string commandName)
        {
            return _commandModuleConfigurations.ContainsKey(commandName ?? "");
        }

        internal bool IsKnownActivityMonitor(string activityMonitorName)
        {
            return _customActivityMonitors.Any(a => a.Name == activityMonitorName);
        }

        internal bool EnableCommandModule(string commandName, bool enable)
        {
            var result = false;

            CommandModuleConfiguration commandConfig;
            if (_commandModuleConfigurations.TryGetValue(commandName, out commandConfig) && commandConfig != null)
            {
                commandConfig.Enabled = enable;
                result = true;
            }

            return result;
        }

        internal bool EnableActivityMonitor(string activityMonitorName, bool enable)
        {
            var result = false;

            var activityMonitor = _customActivityMonitors.FirstOrDefault(a => a.Name == activityMonitorName);
            if (activityMonitor != null)
            {
                activityMonitor.Enabled = enable;
                result = true;
            }

            return result;
        }

        internal bool RenameActivityMonitor(string oldName, string newName)
        {
            var result = false;

            var activityMonitor = _customActivityMonitors.FirstOrDefault(a => a.Name == oldName);
            if (activityMonitor != null)
            {
                activityMonitor.Name = newName;
                result = true;
            }

            return result;
        }

        internal bool CancelTask(string taskId)
        {
            var result = false;

            HyperNodeTaskInfo taskInfo;
            if (_liveTasks.TryGetValue(taskId, out taskInfo) && taskInfo != null)
            {
                taskInfo.Cancel();
                result = true;
            }

            return result;
        }

        #endregion System Commands
    }
}
