using Microsoft.Extensions.DependencyInjection;
using ReportEngine.Import.Services;

namespace ReportApp.Benchmarks;

public static class Program
{
    private static async Task Main(string[] args)
    {
        try
        {
            var jsonFilePath = "C:\\Work\\Исходники квартаскип\\kvartaskip\\export\\stands\\project_4.json";

            using var host = Startup.BuildHost(args, jsonFilePath);

            var projectService = host.Services.GetRequiredService<ProjectService>();

            var stands = projectService.GetStands();

            foreach (var stand in stands) Console.WriteLine(stand.Designation);

            await host.StartAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}