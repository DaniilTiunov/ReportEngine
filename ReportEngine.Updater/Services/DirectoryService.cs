using System.IO;
using ReportEngine.Updater.Config;
using ReportEngine.Updater.Helpers;

namespace ReportEngine.Updater.Services;

public class DirectoryService
{
    private readonly JsonSettingsService _jsonSettingsService;
    
    public DirectoryService(JsonSettingsService jsonSettingsService)
    {
        _jsonSettingsService = jsonSettingsService;
    }
    
    public async Task<IEnumerable<string>> GetDirectoriesAsync(Func<UpdatePaths, string> selector)
    {
        var serverVersionsPath = await _jsonSettingsService.GetPathAsync(
            UpdateSettingsHelper.GetUpdateSettingsPath(),
            selector);

        var ReleasesDirectories = Directory
            .EnumerateDirectories(
                serverVersionsPath,
                "*",
                SearchOption.TopDirectoryOnly);

        return ReleasesDirectories;
    }
    
    public void Copy(
        string sourceDirectory,
        string destinationDirectory)
    {
        var source = new DirectoryInfo(sourceDirectory);

        if (!source.Exists)
            throw new DirectoryNotFoundException(
                $"Исходная папка не найдена: {sourceDirectory}");

        var destination = Path.Combine(
            destinationDirectory,
            source.Name);

        CopyDirectory(source, destination);
    }
    
    private void CopyDirectory(
        DirectoryInfo source,
        string destinationDirectory)
    {
        Directory.CreateDirectory(destinationDirectory);

        foreach (var file in source.GetFiles())
        {
            var destinationFile = Path.Combine(
                destinationDirectory,
                file.Name);

            file.CopyTo(
                destinationFile,
                overwrite: true);
        }

        foreach (var directory in source.GetDirectories())
        {
            var destinationSubDirectory = Path.Combine(
                destinationDirectory,
                directory.Name);

            CopyDirectory(
                directory,
                destinationSubDirectory);
        }
    }
}