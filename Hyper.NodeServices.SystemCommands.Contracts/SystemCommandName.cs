using System.Collections.Generic;
using System.Linq;
using Hyper.NodeServices.Contracts;
using Hyper.NodeServices.Contracts.Extensibility.CommandModules;

namespace Hyper.NodeServices.SystemCommands.Contracts
{
    /// <summary>
    /// Enumerates the names of built-in system command modules.
    /// </summary>
    public static class SystemCommandName
    {
        /// <summary>
        /// Retrieves cached task progress information for the task with the ID specified in the command request string.
        /// <see cref="ICommandRequest"/> type: A <see cref="CommandRequestString"/> containing the ID of the task for which to retrieve cached progress info.
        /// <see cref="ICommandResponse"/> type: A <see cref="GetCachedTaskProgressInfoResponse"/> containing the cached task progress info if the cache is enabled. Otherwise, returns a warning indicating that the cache is disabled.
        /// </summary>
        public const string GetCachedTaskProgressInfo     = "GetCachedTaskProgressInfo";

        /// <summary>
        /// Retrieves the names of all <see cref="IHyperNodeService"/> instances that are immediate children of the intended recipient.
        /// <see cref="ICommandRequest"/> type: No request type is required for this command module.
        /// <see cref="ICommandResponse"/> type: A <see cref="GetChildNodesResponse"/> containing the immediate children of the intended recipient.
        /// </summary>
        public const string GetChildNodes                 = "GetChildNodes";

        /// <summary>
        /// Retrieves the status of the <see cref="IHyperNodeService"/>.
        /// <see cref="ICommandRequest"/> type: No request type is required for this command module.
        /// <see cref="ICommandResponse"/> type: A <see cref="GetNodeStatusResponse"/> containing a status summary of the intended recipient, including
        /// lists of active tasks, commands, and custom activity monitors as well as the current settings for the <see cref="IHyperNodeService"/>.
        /// </summary>
        public const string GetNodeStatus                 = "GetNodeStatus";

        /// <summary>
        /// Echoes the request string back to the caller. This command is useful for checking <see cref="IHyperNodeService"/> connectivity and
        /// command request/response passthrough.
        /// <see cref="ICommandRequest"/> type: A <see cref="CommandRequestString"/> containing the string to echo back to the caller.
        /// <see cref="ICommandResponse"/> type: A <see cref="CommandResponseString"/> containing a slightly modified version of the request string.
        /// </summary>
        public const string Echo                          = "Echo";

        /// <summary>
        /// Enables or disables the command module with the name specified in the request object.
        /// <see cref="ICommandRequest"/> type: An <see cref="EnableCommandModuleRequest"/> containing the command name and whether to enable or disable it.
        /// <see cref="ICommandResponse"/> type: A <see cref="CommandResponse"/> indicating success or failure.
        /// </summary>
        public const string EnableCommand                 = "EnableCommand";

        /// <summary>
        /// Enables or disables the custom activity monitor with the name specified in the request object.
        /// <see cref="ICommandRequest"/> type: An <see cref="EnableActivityMonitorRequest"/> containing the monitor name and whether to enable or disable it.
        /// <see cref="ICommandResponse"/> type: A <see cref="CommandResponse"/> indicating success or failure.
        /// </summary>
        public const string EnableActivityMonitor         = "EnableActivityMonitor";

        /// <summary>
        /// Renames the custom activity monitor with the name specified in the request object.
        /// <see cref="ICommandRequest"/> type: A <see cref="RenameActivityMonitorRequest"/> containing the old name and the new name of the custom activity monitor.
        /// <see cref="ICommandResponse"/> type: A <see cref="CommandResponse"/> indicating success or failure.
        /// </summary>
        public const string RenameActivityMonitor         = "RenameActivityMonitor";

        /// <summary>
        /// Enables or disables the task progress cache.
        /// <see cref="ICommandRequest"/> type: An <see cref="EnableTaskProgressCacheRequest"/> indicating whether or enable or disable the task progress cache.
        /// <see cref="ICommandResponse"/> type: A <see cref="CommandResponse"/> indicating success or failure.
        /// </summary>
        public const string EnableTaskProgressCache       = "EnableTaskProgressCache";

        /// <summary>
        /// Enables or disables diagnostics.
        /// <see cref="ICommandRequest"/> type: An <see cref="EnableDiagnosticsRequest"/> indicating whether or enable or disable diagnostics.
        /// <see cref="ICommandResponse"/> type: A <see cref="CommandResponse"/> indicating success or failure.
        /// </summary>
        public const string EnableDiagnostics             = "EnableDiagnostics";

        /// <summary>
        /// Cancels the task with the ID specified in the command request string.
        /// <see cref="ICommandRequest"/> type: A <see cref="CommandRequestString"/> containing the ID of the task to cancel.
        /// <see cref="ICommandResponse"/> type: A <see cref="CommandResponse"/> indicating success or failure.
        /// </summary>
        public const string CancelTask                    = "CancelTask";

        /// <summary>
        /// Sets the sliding expiration time for items in the task progress cache.
        /// <see cref="ICommandRequest"/> type: A <see cref="SetTaskProgressCacheDurationRequest"/> specifying how long the duration should be.
        /// <see cref="ICommandResponse"/> type: A <see cref="SetTaskProgressCacheDurationResponse"/> indicating success or failure. A flag is returned indicating whether the cache is enabled.
        /// </summary>
        public const string SetTaskProgressCacheDuration  = "SetTaskProgressCacheDuration";

        private static readonly string[] SystemCommands =
        {
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

        /// <summary>
        /// Tests whether the specified command name is a system command.
        /// </summary>
        /// <param name="commandName">The command name to check.</param>
        /// <returns></returns>
        public static bool IsSystemCommand(string commandName)
        {
            return SystemCommands.Contains(commandName);
        }

        /// <summary>
        /// Returns an <see cref="IEnumerable{T}"/> containing all system command names.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<string> GetAll()
        {
            return SystemCommands;
        }
    }
}
