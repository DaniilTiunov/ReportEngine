using System.IO;

namespace ReportEngine.App.Config.Directory
{
    public class DirectoryHelper
    {
        public static string GetDirectory()
        {
            return AppDomain.CurrentDomain.BaseDirectory; ;
        }
        public static string GetConfigPath(string appDirectory)
        {
            return Path.Combine(appDirectory, "Config", "appsettings.json");
        }
        public static string GetLogsPath(string appDirectory)
        {
            return Path.Combine(appDirectory, "logs", "log.txt");
        }
    }
}
