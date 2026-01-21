using System.Globalization;
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
            SetCulture();

            Log.Logger = LoggerConfig.InitializeLogger();

            var connString = JsonHandler.GetConnectionString(DirectoryHelper.GetConfigPath());

            var host = HostFactory.BuildHost(connString);

            var app = host.Services.GetRequiredService<App>();

            var mainWindow = host.Services.GetRequiredService<MainWindow>();
            app.MainWindow = mainWindow;

            mainWindow.Show();

            host.StartAsync();

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

    public static void SetCulture()
    {
        CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("ru-RU");
        CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("ru-RU");
        Thread.CurrentThread.CurrentCulture = new CultureInfo("ru-RU");
        Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru-RU");
    }
}
