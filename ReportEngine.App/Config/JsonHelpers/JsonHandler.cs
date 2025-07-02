using System;
using System.IO;
using System.Text.Json;

namespace ReportEngine.App.Config.JsonHelpers
{
    public class JsonHandler
    {
        private string _jsonFileName { get; set; }
        private AppSettings _appSettings { get; set; }

        public JsonHandler(string jsonFileName)
        {
            _jsonFileName = jsonFileName;
            string json = File.ReadAllText(_jsonFileName);
            _appSettings = JsonSerializer.Deserialize<AppSettings>(json);
        }

        public string GetConnectionString()
        {
            return _appSettings.ConnectionStrings.DefaultConnection;
        }
    }
}
