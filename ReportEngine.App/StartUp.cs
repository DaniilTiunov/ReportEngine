using System.Globalization;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using ReportEngine.App.Services.Notification;
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

            Thread.Sleep(1000);

            app.MainWindow = mainWindow;

            mainWindow.Show();

            Log.Information("Приложение запущено");

            app.Run();
        }
        catch (Exception ex)
        {
            ShowErrorWindow(ex.Message);

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

    private static void ShowErrorWindow(string errorMessage)
    {
        try
        {
            MessageBox.Show(
                errorMessage,
                "Ошибка запуска",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"КРИТИЧЕСКАЯ ОШИБКА: {errorMessage}");
            Console.WriteLine($"Ошибка при показе окна: {ex.Message}");
        }
    }
}
