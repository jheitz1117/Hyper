﻿using System.Linq;

namespace Hyper.NodeServices.Contracts
{
    public static class SystemCommandNames
    {
        public const string Discover                  = "Discover";
        public const string GetCachedTaskProgressInfo = "GetCachedTaskProgressInfo";
        public const string GetChildNodes             = "GetChildNodes";
        public const string GetCommandConfig          = "GetCommandConfig";
        public const string Echo                      = "Echo";
        public const string EnableCommand             = "EnableCommand";
        public const string EnableActivityMonitor     = "EnableActivityMonitor";
        public const string RenameActivityMonitor     = "RenameActivityMonitor";
        public const string EnableActivityCache       = "EnableActivityCache";
        public const string EnableDiagnostics         = "EnableDiagnostics";
        public const string CancelTask                = "CancelTask";

        private static readonly string[] SystemCommands =
        {
            Discover,
            GetCachedTaskProgressInfo,
            GetChildNodes,
            GetCommandConfig,
            Echo,
            EnableCommand,
            EnableActivityMonitor,
            RenameActivityMonitor,
            EnableActivityCache,
            EnableDiagnostics,
            CancelTask
        };

        public static bool IsSystemCommand(string commandName)
        {
            return SystemCommands.Contains(commandName);
        }
    }
}
