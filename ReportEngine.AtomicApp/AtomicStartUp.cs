using Microsoft.Extensions.DependencyInjection;
using ReportEngine.AtomicApp.Views;
using ReportEngine.Shared.Config.Directory;
using ReportEngine.Shared.Config.JsonHelpers;

namespace ReportEngine.AtomicApp;

internal static class AtomicStartUp
{
    [STAThread]
    public static void Main()
    {
        var connectionString = JsonHandler.GetAtomicConnectionString(DirectoryHelper.GetConfigPath());

        var host = AtomicHostFactory.BuildHost(connectionString);

        var app = host.Services.GetService<App>();
        var mainWindow = host.Services.GetService<MainWindow>();

        mainWindow.Show();
        app.Run();
    }
}
