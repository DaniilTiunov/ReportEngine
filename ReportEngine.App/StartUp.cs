using System.Runtime.InteropServices;
using Microsoft.Extensions.DependencyInjection;
using ReportEngine.Shared.Config.Directory;
using ReportEngine.Shared.Config.JsonHelpers;
using ReportEngine.Shared.Config.Logger;
using Serilog;

namespace ReportEngine.App;

public class StartUp
{
    [STAThread]
    public static void Main()
    {
        try
        {
#if DEBUG
            AllocConsole();
#endif
            Log.Logger = LoggerConfig.InitializeLogger();

            var connString = JsonHandler.GetConnectionString(DirectoryHelper.GetConfigPath());

            var host = HostFactory.BuildHost(connString);

            var app = host.Services.GetRequiredService<App>();

            var mainWindow = host.Services.GetRequiredService<MainWindow>();
            app.MainWindow = mainWindow;
            mainWindow.Show();

            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Приложение не запущено");
            throw; // Пробрасываем исключение дальше для отладки
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    [DllImport("kernel32.dll")]
    private static extern bool AllocConsole();
}