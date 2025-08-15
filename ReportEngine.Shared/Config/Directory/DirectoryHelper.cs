namespace ReportEngine.Shared.Config.Directory;

public class DirectoryHelper
{
    public static string GetDirectory()
    {
        return AppDomain.CurrentDomain.BaseDirectory;
        ;
    }

    public static string GetConfigPath()
    {
        return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Config", "appsettings.json");
    }

    public static string GetLogsPath()
    {
        return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "log.txt");
    }

    public static string GetImagesPath(string imageName)
    {
        return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Images", "Obvyazka",
            imageName + ".jpg");
    }
}