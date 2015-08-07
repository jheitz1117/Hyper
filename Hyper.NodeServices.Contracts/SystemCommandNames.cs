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

        private static readonly string[] SystemCommands =
        {
            Discover,
            GetCachedTaskProgressInfo,
            GetChildNodes,
            GetCommandConfig,
            Echo,
            EnableCommand
        };

        public static bool IsSystemCommand(string commandName)
        {
            return SystemCommands.Contains(commandName);
        }
    }
}
