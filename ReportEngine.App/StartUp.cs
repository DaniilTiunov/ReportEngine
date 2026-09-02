using System.Globalization;
using System.Text;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using ReportEngine.App.Views.Windows.Dialog;
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
            
            var splash = new SplashWindow();
            splash.Show();

            // Шаг 1
            splash.SetStatusText("Загрузка файлов конфигурации...");
            Thread.Sleep(500);
            
            var config = JsonHandler.GetDatabaseMode(DirectoryHelper.GetConfigPath());

            // Шаг 2
            splash.SetStatusText("Сборка хоста...");
            Thread.Sleep(500);
            
            var host = HostFactory.BuildHost(config);

            // Шаг 3
            splash.SetStatusText("Регистрация контекста данных...");
            Thread.Sleep(500);
            
            var context = host.Services.GetRequiredService<ReAppContext>();
            var app = host.Services.GetRequiredService<App>();

            // Шаг 4
            splash.CheckDbStatus(context);
            Thread.Sleep(500);
            
            CanConnect = CheckDbConnection(context);

            if (CanConnect)
            {
                try
                {
                    splash.SetStatusText("Загрузка необходимых данных из базы данных...");
                    Thread.Sleep(500);
                    
                    host.Services
                        .GetRequiredService<ParametersStore>()
                        .LoadSettingsDataAsync()
                        .GetAwaiter()
                        .GetResult();
                }
                catch (Exception ex)
                {
                    Log.Fatal(ex, "Ошибка загрузки ParameterStore");
                }
            }

            splash.SetStatusText("Запуск приложения...");

            Thread.Sleep(500);
            var mainWindow = host.Services.GetRequiredService<MainWindow>();
            app.MainWindow = mainWindow;

            mainWindow.Show();
            splash.Close();

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
