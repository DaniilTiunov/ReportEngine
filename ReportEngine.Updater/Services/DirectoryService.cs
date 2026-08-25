using System.IO;

namespace ReportEngine.Updater.Services;

public class DirectoryService
{
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