using System.Text.Json;
using ReportEngine.Extensions.Extensions;
using ReportEngine.Import.Models;

namespace ReportEngine.Import.Services;

public class JsonFileService
{
    private string _filePath;

    public JsonFileService(string filePath)
    {
        _filePath = filePath;
    }
    
    public async Task<ProjectFile> GetProjectFileDataAsync()
    {
        var fileContent = await ReadFileAsync();
        
        var options = new JsonSerializerOptions().SetOptions();
        
        return JsonSerializer.Deserialize<ProjectFile>(fileContent, options) ??  new ProjectFile();
    }
    
    private async Task<string> ReadFileAsync()
    {
        var fileContent = await File.ReadAllTextAsync(_filePath);
        return fileContent;
    }
}