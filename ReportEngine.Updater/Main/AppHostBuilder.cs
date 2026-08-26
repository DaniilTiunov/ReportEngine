using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ReportEngine.Updater.Config;
using ReportEngine.Updater.Services;
using ReportEngine.Updater.ViewModels;
using ReportEngine.Updater.Views;
using ReportEngine.Updater.Views.Dialog;

namespace ReportEngine.Updater.Main;

public class AppHostBuilder
{
    public static IHost Build()
    {
        return Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                // Настройка JSON опций
                services.AddSingleton<JsonSerializerOptions>(provider =>
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                        WriteIndented = true,
                        Converters = 
                        { 
                            new JsonStringEnumConverter() 
                        }
                    };
                    return options;
                });

                ConfigureAppServices(services);
                ConfigureViews(services);
                ConfigureViewModels(services);
            })
            .Build();
    }

    private static void ConfigureAppServices(IServiceCollection services)
    {
        services.AddSingleton<App>();
        services.AddSingleton<NotificationService>();
        services.AddScoped<JsonSettingsService>();
        services.AddScoped<DirectoryService>();
        services.AddScoped<UpdateService>();
    }

    private static void ConfigureViews(IServiceCollection services)
    {
        services.AddSingleton<MainWindow>();
        services.AddTransient<HomeView>();
        services.AddTransient<VersionsView>();
        services.AddTransient<SettingsView>();
        services.AddTransient<NotifyWindow>();
    }

    private static void ConfigureViewModels(IServiceCollection services)
    {
        services.AddScoped<MainWindowViewModel>();
        services.AddScoped<SettingsViewModel>();
        services.AddScoped<VersionsViewModel>();
    }
}