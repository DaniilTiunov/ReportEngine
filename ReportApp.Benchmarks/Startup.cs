using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReportEngine.Domain.Database.Context;
using ReportEngine.Domain.Repositories;

namespace ReportApp.Benchmarks;

public static class Startup
{
    public static IHost BuildHost(string[] args, string filePath)
    {
        
        
        return Host
            .CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                ConfigureDbContext(services);
            })
            .Build();
    }

    private static void ConfigureRepositiries(IServiceCollection services)
    {
        services.AddScoped<ProjectInfoRepository>();
    }
    
    private static void ConfigureDbContext(IServiceCollection services)
    {
        services.AddDbContext<ReAppContext>(options =>
        {
            options.UseNpgsql(
                connectionString: "Host=172.16.10.230;Port=5432;Database=reportengine;Username=postgres;Password=postgres");
        });
    }
}