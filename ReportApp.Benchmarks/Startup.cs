using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReportEngine.Import.Services;

namespace ReportApp.Benchmarks;

public static class Startup
{
    public static IHost BuildHost(string[] args, string filePath)
    {
        var jsonService = new JsonFileService(filePath);
        
        return Host
            .CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                services.AddSingleton(jsonService);
                services.AddSingleton<ProjectService>();
            })
            .Build();
    }
}