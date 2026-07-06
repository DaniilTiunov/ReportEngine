using System.Text.Json;

namespace ReportEngine.Shared.Config.JsonHelpers;

public class JsonHandler
{
    public static string GetAtomicConnectionString(string jsonFilePath)
    {
        var json = File.ReadAllText(jsonFilePath);
        var appSettings = JsonSerializer.Deserialize<AppSettings>(json);
        return appSettings.AtomicConnectionString.AtomicDefaultConnection;
    }

    public static string GetConnectionString(string jsonFilePath)
    {
        var json = File.ReadAllText(jsonFilePath);
        var appSettings = JsonSerializer.Deserialize<AppSettings>(json);
        return appSettings.ConnectionStrings.DefaultConnection;
    }

    public static string GetSqlLiteConnection(string jsonFilePath)
    {
        var json = File.ReadAllText(jsonFilePath);
        var appSettings = JsonSerializer.Deserialize<AppSettings>(json);
        return appSettings.ConnectionStrings.SqliteConnectionString;
    }

    public static string GetDatabaseMode(string jsonFilePath)
    {
        var json = File.ReadAllText(jsonFilePath);
        var appSettings = JsonSerializer.Deserialize<AppSettings>(json);
        return appSettings.DatabaseSettings.DatabaseMode;
    }

    public static void SetDatabaseMode(string jsonFilePath, string newDatabaseMode)
    {
        var json = File.ReadAllText(jsonFilePath);
        var appSettings = JsonSerializer.Deserialize<AppSettings>(json);
        appSettings.DatabaseSettings.DatabaseMode = newDatabaseMode;
        var newJson = JsonSerializer.Serialize(appSettings, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(jsonFilePath, newJson);
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
