using System.IO;
using System.Text.Json;

namespace ReportEngine.Shared.Config.JsonHelpers
{
    public class JsonHandler
    {
        public static string GetConnectionString(string jsonFilePath)
        {
            string json = File.ReadAllText(jsonFilePath);
            var appSettings = JsonSerializer.Deserialize<AppSettings>(json);
            return appSettings.ConnectionStrings.DefaultConnection;
        }
        public static string GetLocalVersion(string jsonFilePath)
        {
            string json = File.ReadAllText(jsonFilePath);
            var appSettings = JsonSerializer.Deserialize<AppSettings>(json);
            return appSettings.AboutProgram.Version;
        }
        public static string GetVersionOnServer(string jsonFilePath)
        {
            string json = File.ReadAllText(jsonFilePath);
            var appSettings = JsonSerializer.Deserialize<AppSettings>(json);
            return appSettings.AboutProgram.VersionOnServerPath;
        }
        public static string GetCurrentVersion(string jsonFilePath)
        {
            string json = File.ReadAllText(jsonFilePath);
            var appSettings = JsonSerializer.Deserialize<AppSettings>(json);
            return appSettings.AboutProgram.Version;
        }
    }
}
