using System;

namespace WebEssentials
{
    public class Constants
    {
        public const string LiveFeedUrl = "https://raw.githubusercontent.com/madskristensen/WebEssentials2017/master/extensions.json";//"https://cdn.rawgit.com/madskristensen/WebEssentials2017/master/extensions.json";
        public static readonly string LiveFeedCachePath = Environment.ExpandEnvironmentVariables("%localAppData%\\" + Vsix.Name + "\\feed.json");
        public static readonly string LogFile = Environment.ExpandEnvironmentVariables("%localAppData%\\" + Vsix.Name + "\\installer.log");

        public const double UpdateIntervalDays = 1;
        public const string RegistrySubKey = "WebEssentials2017";

#if DEBUG
        public static bool IsDebug = true;
#else
          public static bool IsDebug = false;
#endif
    }
}
