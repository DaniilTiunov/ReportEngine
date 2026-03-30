using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReportEngine.AtomicApp.Services;
using ReportEngine.AtomicApp.ViewModels;
using ReportEngine.AtomicApp.Views;
using ReportEngine.AtomicDomain.Database.Context;
using ReportEngine.AtomicDomain.Repositories;
using ReportEngine.AtomicServices.Services;

namespace ReportEngine.AtomicApp;

public static class AtomicHostFactory
{
    public static IHost BuildHost(string connectionString)
    {
        return Host.CreateDefaultBuilder()
            .ConfigureServices((hostContext, services) =>
            {
                ConfigureDatabase(services, connectionString);
                ConfigureViews(services);
                ConfigureServices(services);
                ConfigureRepositories(services);
                ConfigureViewModel(services);

                services.AddSingleton<App>();
                services.AddSingleton<MainWindow>();
            })
            .Build();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddScoped<AtomicProjectService>();
    }

    private static void ConfigureRepositories(IServiceCollection services)
    {
        services.AddScoped<AtomicProjectRepository>();
    }

    private static void ConfigureDatabase(IServiceCollection services, string connString)
    {
        services.AddDbContext<AtomicAppContext>(options =>
            options.UseNpgsql(connString));
    }

    private static void ConfigureViewModel(IServiceCollection services)
    {
        services.AddScoped<AtomicProjectViewModel>();
    }

    private static void ConfigureViews(IServiceCollection services)
    {
        services.AddSingleton<MainWindow>();
        services.AddSingleton(provider =>
        {
            var navService = provider.GetRequiredService<NavigationService>();
            var viewModel = provider.GetRequiredService<AtomicMainWindowViewModel>();
            var mainWindow = new MainWindow(viewModel, provider);

            navService.InitializeContentHost(mainWindow.FindName("MainContentControl") as ContentControl);
            return mainWindow;
        });
    }
}
