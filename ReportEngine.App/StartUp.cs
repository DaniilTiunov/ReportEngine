using Microsoft.Extensions.DependencyInjection;
using ReportEngine.App.ViewModels;
using ReportEngine.App.Views;
using ReportEngine.Shared.Config.DebugConsol;
using ReportEngine.Shared.Config.Directory;
using ReportEngine.Shared.Config.JsonHelpers;
using ReportEngine.Shared.Config.Logger;
using Serilog;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace ReportEngine.App
{
    public class StartUp
    {
        [STAThread]
        public static void Main()
        {
            try
            {
#if DEBUG
                [DllImport("kernel32.dll")]
                static extern bool AllocConsole();
                AllocConsole();
#endif

                DebugConsole.WriteLine("Тест консоли");

                var connString = JsonHandler.GetConnectionString(DirectoryHelper.GetConfigPath());// Получаем строку подключения из json файла

                Log.Logger = LoggerConfig.InitializeLogger(); // Конфигурация Serilog

                var host = HostFactory.BuildHost(connString); //Конфигурация Host приложения 

                var app = host.Services.GetService<App>(); //Получаем экземпляр приложения 

                var mainWindow = host.Services.GetService<MainWindow>(); //Получаем экземпляр окна

                app.MainWindow = mainWindow;
                mainWindow?.Show(); //Показываем главное окно приложения
                app?.Run(); //Запускаем


                Log.Information("Приложение запущено без ошибок"); //Логируем
                Log.Verbose("Это Verbose сообщение");
                Log.Debug("Это Debug сообщение");
                Log.Information("Это Information сообщение");
                Log.Warning("Это Warning сообщение");
                Log.Error("Это Error сообщение");
                Log.Fatal("Это Fatal сообщение");
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Приложение не запущено");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
