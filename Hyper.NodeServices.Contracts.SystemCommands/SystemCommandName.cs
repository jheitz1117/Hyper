using System.Linq;
using Hyper.NodeServices.Contracts.Extensibility.CommandModules;

namespace Hyper.NodeServices.Contracts.SystemCommands
{
    /// <summary>
    /// Enumerates the names of built-in system command modules.
    /// </summary>
    public static class SystemCommandName
    {
        /// <summary>
        /// Reports the names of all HyperNode descendants.
        /// <see cref="ICommandRequest"/> type: No request type is required for this command module.
        /// <see cref="ICommandResponse"/> type: <see cref="DiscoverResponse"/>.
        /// </summary>
        public const string Discover                      = "Discover";

        /// <summary>
        /// Retrieves cached task progress information for the task with the ID specified in the command request string.
        /// <see cref="ICommandRequest"/> type: <see cref="CommandRequestString"/> containing the ID of the task for which to retrieve cached progress info.
        /// <see cref="ICommandResponse"/> type: <see cref="GetCachedTaskProgressInfoResponse"/>.
        /// </summary>
        public const string GetCachedTaskProgressInfo     = "GetCachedTaskProgressInfo";
        public const string GetChildNodes                 = "GetChildNodes";
        public const string GetNodeStatus                 = "GetNodeStatus";
        public const string Echo                          = "Echo";
        public const string EnableCommand                 = "EnableCommand";
        public const string EnableActivityMonitor         = "EnableActivityMonitor";
        public const string RenameActivityMonitor         = "RenameActivityMonitor";
        public const string EnableTaskProgressCache       = "EnableTaskProgressCache";
        public const string EnableDiagnostics             = "EnableDiagnostics";
        public const string CancelTask                    = "CancelTask";
        public const string SetTaskProgressCacheDuration  = "SetTaskProgressCacheDuration";

        private static readonly string[] SystemCommands =
        {
            Discover,
            GetCachedTaskProgressInfo,
            GetChildNodes,
            GetNodeStatus,
            Echo,
            EnableCommand,
            EnableActivityMonitor,
            RenameActivityMonitor,
            EnableTaskProgressCache,
            EnableDiagnostics,
            CancelTask,
            SetTaskProgressCacheDuration
        };

        public static bool IsSystemCommand(string commandName)
        {
            return SystemCommands.Contains(commandName);
        }
    }
}
