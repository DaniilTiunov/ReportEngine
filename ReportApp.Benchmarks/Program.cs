namespace ReportApp.Benchmarks;

public static class Program
{
    private static async Task Main(string[] args)
    {
        try
        {
            var jsonFilePath = "C:\\Work\\Исходники квартаскип\\kvartaskip\\export\\stands\\project_4.json";

            using var host = Startup.BuildHost(args, jsonFilePath);


            await host.StartAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}