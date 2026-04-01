using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using ReportEngine.Domain.Store;
using ReportEngine.Shared.Config.Directory;
using ReportEngine.Shared.Config.JsonHelpers;
using ReportEngine.Shared.Config.Logger;
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

            Log.Logger = LoggerConfig.InitializeLogger();

            var connString = JsonHandler.GetConnectionString(DirectoryHelper.GetConfigPath());

            var host = HostFactory.BuildHost(connString);

            var app = host.Services.GetRequiredService<App>();

            var mainWindow = host.Services.GetRequiredService<MainWindow>();

            var parametersStore = host.Services.GetRequiredService<ParametersStore>();

            parametersStore.LoadSettingsDataAsync();

            app.MainWindow = mainWindow;

            mainWindow.Show();
            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Приложение не запущено");
            throw;
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
