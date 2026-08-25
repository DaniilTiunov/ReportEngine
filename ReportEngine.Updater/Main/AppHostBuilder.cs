using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReportEngine.Updater.Config;
using ReportEngine.Updater.Services;
using ReportEngine.Updater.ViewModels;
using ReportEngine.Updater.Views;

namespace ReportEngine.Updater.Main;

public class AppHostBuilder
{
    public static IHost Build()
    {
        return Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                ConfigureAppServices(services);
                ConfigureViews(services);
                ConfigureViewModels(services);
            })
            .Build();
    }

    private static void ConfigureAppServices(IServiceCollection services)
    {
        services.AddSingleton<App>();
        services.AddScoped<UpdateSettingsService>();
        services.AddScoped<DirectoryService>();
    }

    private static void ConfigureViews(IServiceCollection services)
    {
        services.AddSingleton<MainWindow>();
        services.AddTransient<HomeView>();
        services.AddTransient<VersionsView>();
        services.AddTransient<SettingsView>();
    }

    private static void ConfigureViewModels(IServiceCollection services)
    {
        services.AddScoped<MainWindowViewModel>();
        services.AddScoped<SettingsViewModel>();
        services.AddScoped<VersionsViewModel>();
    }
}