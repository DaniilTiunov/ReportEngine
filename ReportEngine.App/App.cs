using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using ReportEngine.App.Services.Theming;
using ReportEngine.App.Views;

namespace ReportEngine.App;

public class App : Application
{
    private readonly IServiceProvider _services;

    public App(IServiceProvider services, IThemeService themeService)
    {
        _services = services;
        themeService.ApplyTheme(AppTheme.Light);
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        MainWindow = _services.GetRequiredService<MainWindow>();
        MainWindow.Show();
    }
}
