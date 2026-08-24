using Microsoft.Extensions.DependencyInjection;
using ReportEngine.Updater.Views;

namespace ReportEngine.Updater.Main;

public static class Startup
{
    [STAThread]
    public static void Main()
    {
        var host = AppHostBuilder.Build();

        var app = host.Services.GetRequiredService<App>();
        
        var mainWindow = host.Services.GetRequiredService<MainWindow>();
        
        mainWindow.Show();
        app.Run();
        
    }
}