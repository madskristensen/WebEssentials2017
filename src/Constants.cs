using System;

namespace WebEssentials
{
    public class Constants
    {
        public const string LiveFeedUrl = "https://aka.ms/hj6y82";
        public static readonly string LiveFeedCachePath = Environment.ExpandEnvironmentVariables("%localAppData%\\" + Vsix.Name + "\\feed.json");
        public static readonly string LogFile = Environment.ExpandEnvironmentVariables("%localAppData%\\" + Vsix.Name + "\\installer.log");

        public const double UpdateIntervalDays = 1;
        public const string RegistrySubKey = "WebEssentials2017";
    }
}
