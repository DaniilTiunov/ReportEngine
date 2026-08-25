using System.IO;

namespace ReportEngine.Updater.Helpers;

public class UpdateSettingsHelper
{
    private static string GetDirectory()
    {
        return AppDomain.CurrentDomain.BaseDirectory;
    }

    public static string GetUpdateSettingsPath()
    {
        return Path.Combine(GetDirectory(), "Config", "updateSettings.json");
    }
}