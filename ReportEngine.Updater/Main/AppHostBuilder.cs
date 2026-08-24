using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
        services.AddSingleton<MainWindow>();
        services.AddSingleton<App>();
    }

    private static void ConfigureViews(IServiceCollection services)
    {
        services.AddTransient<HomeView>();
        services.AddTransient<VersionsView>();
        services.AddTransient<SettingsView>();
    }

    private static void ConfigureViewModels(IServiceCollection services)
    {
        services.AddScoped<MainWindowViewModel>();
    }
}