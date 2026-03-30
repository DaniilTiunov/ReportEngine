using Microsoft.Extensions.DependencyInjection;
using ReportEngine.Launcher.Views;

namespace ReportEngine.Launcher;

public static class StartUpLauncher
{
    [STAThread]
    public static void Main()
    {
        var host = HostFactoryLauncher.BuildHost();

        var app = host.Services.GetService<App>();
        var mainWindow = host.Services.GetService<MainWindow>();


        mainWindow.Show();
        app.Run();
    }
}
