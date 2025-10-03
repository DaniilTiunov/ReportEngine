namespace ReportEngine.Shared.Config.Directory;

public class DirectoryHelper
{
    public static string GetDirectory()
    {
        return AppDomain.CurrentDomain.BaseDirectory;
    }

    public static string GetConfigPath()
    {
        return Path.Combine(GetDirectory(), "Config", "appsettings.json");
    }

    public static string GetIniConfigPath()
    {
        return Path.Combine(GetDirectory(), "Config", "settings.ini");
    }

    public static string GetLogsPath()
    {
        return Path.Combine(GetDirectory(), "logs", "log.txt");
    }

    public static string GetImagesPath(string imageName)
    {
        return Path.Combine(GetDirectory(), "Resources", "Images", "Obvyazka",
            imageName + ".jpg");
    }

    public static string GetImagesRootPath(string imageName)
    {
        return Path.Combine(GetDirectory(), "Resources", "Images",
            imageName + ".jpg");
    }

    public static string GetReportsTemplatePath(string templateName, string fileFormat)
    {
        return Path.Combine(GetDirectory(), "ReportTemplates", templateName + fileFormat);
    }

    public static string GetReportsDirectory()
    {
        return Path.Combine(GetDirectory(), "Отчёты");
    }

    public static string GetReportSavePath(string fileName)
    {
        var dir = GetReportsDirectory();

        return Path.Combine(dir, fileName);
    }


}