using System;
using System.IO;
using System.Text.Json;

namespace ReportEngine.App.Config.JsonHelpers
{
    public class JsonHandler
    {
        private AppSettings _appSettings { get; set; }
        public static string GetConnectionString(string jsonFilePath)
        {
            string json = File.ReadAllText(jsonFilePath);
            var appSettings = JsonSerializer.Deserialize<AppSettings>(json);
            return appSettings.ConnectionStrings.DefaultConnection;
        }
    }
}
