using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using ReportEngine.Domain.Store;
using ReportEngine.Shared.Config.Directory;
using ReportEngine.Shared.Config.JsonHelpers;
using Serilog;

namespace ReportEngine.App;

public static class StartUp
{
    [STAThread]
    public static void Main()
    {
        try
        {
            SetCulture();

            var config = JsonHandler.GetDatabaseMode(DirectoryHelper.GetConfigPath());

            var host = HostFactory.BuildHost(config);

            var app = host.Services.GetRequiredService<App>();

            var mainWindow = host.Services.GetRequiredService<MainWindow>();

            var parametersStore = host.Services.GetRequiredService<ParametersStore>();

            parametersStore.LoadSettingsDataAsync().GetAwaiter().GetResult();

            app.MainWindow = mainWindow;

            mainWindow.Show();

            Log.Information("Приложение запущено");

            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal($"Ошибка запуска {ex.Message}");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    public static void SetCulture()
    {
        var culture = new CultureInfo("ru-RU");

        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
        Thread.CurrentThread.CurrentCulture = culture;
        Thread.CurrentThread.CurrentUICulture = culture;
    }
}
