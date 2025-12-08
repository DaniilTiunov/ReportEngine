using System.Text.Json;

namespace ReportEngine.Shared.Config.JsonHelpers;

public class JsonHandler
{
    public static string GetConnectionString(string jsonFilePath)
    {
        var json = File.ReadAllText(jsonFilePath);
        var appSettings = JsonSerializer.Deserialize<AppSettings>(json);
        return appSettings.ConnectionStrings.DefaultConnection;
    }

    public static void SetConnectionString(string jsonFilePath, string newConnectionString)
    {
        var json = File.ReadAllText(jsonFilePath);
        var appSettings = JsonSerializer.Deserialize<AppSettings>(json);
        appSettings.ConnectionStrings.DefaultConnection = newConnectionString;
        var newJson = JsonSerializer.Serialize(appSettings, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(jsonFilePath, newJson);
    }

    public static string GetCurrentVersion(string jsonFilePath)
    {
        var json = File.ReadAllText(jsonFilePath);
        var appSettings = JsonSerializer.Deserialize<AppSettings>(json);
        return appSettings.AboutProgram.Version;
    }
}
