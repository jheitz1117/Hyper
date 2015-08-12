using System.Linq;

namespace Hyper.NodeServices.Contracts
{
    public static class SystemCommandNames
    {
        public const string Discover                      = "Discover";
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
