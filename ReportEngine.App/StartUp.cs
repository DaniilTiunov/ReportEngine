using System.Globalization;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using ReportEngine.Domain.Database.Context;
using ReportEngine.Domain.Store;
using ReportEngine.Shared.Config.Directory;
using ReportEngine.Shared.Config.JsonHelpers;
using Serilog;

namespace ReportEngine.App;

public static class StartUp
{
    private static Mutex _mutex;

    public static bool CanConnect;

    [STAThread]
    public static void Main()
    {
        _mutex = new Mutex(true, "Global\\ReportEngineApp", out var createdNew);

        if (!createdNew)
        {
            MessageBox.Show("Приложение уже запущено", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        try
        {
            SetCulture();

            SplashManager.Start();

            SplashManager.SetStatus(
                "Загрузка файлов конфигурации...");

            var config = JsonHandler.GetDatabaseMode(
                DirectoryHelper.GetConfigPath());

            SplashManager.SetStatus(
                "Сборка хоста...");

            var host = HostFactory.BuildHost(config);

            SplashManager.SetStatus(
                "Регистрация контекста данных...");

            var context =
                host.Services.GetRequiredService<ReAppContext>();
            
            SplashManager.SetStatus(
                "Инициализация приложения...");

            var app =
                host.Services.GetRequiredService<App>();

            SplashManager.SetStatus(
                "Проверка подключения к БД...");

            CanConnect = context.Database.CanConnect();

            if (CanConnect)
            {
                SplashManager.SetStatus(
                    "Загрузка необходимых данных из базы данных...");

                host.Services
                    .GetRequiredService<ParametersStore>()
                    .LoadSettingsDataAsync()
                    .GetAwaiter()
                    .GetResult();
            }

            SplashManager.SetStatus(
                "Запуск приложения...");

            var mainWindow =
                host.Services.GetRequiredService<MainWindow>();

            app.MainWindow = mainWindow;

            mainWindow.Show();

            SplashManager.Close();

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
            ReleaseMutex();
            DisposeMutex();
            Log.CloseAndFlush();
        }
    }

    private static bool CheckDbConnection(ReAppContext context)
    {
        var canConnect = context.Database.CanConnect();

        if (!canConnect) ShowErrorWindow("Отсутствует подключение к БД");

        return canConnect;
    }

    private static void SetCulture()
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


    public static void ReleaseMutex()
    {
        _mutex?.ReleaseMutex();
    }

    public static void DisposeMutex()
    {
        if (_mutex != null)
        {
            _mutex.Close();
            _mutex.Dispose();
            _mutex = null;
        }
    }
}