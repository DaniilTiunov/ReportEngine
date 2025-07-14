using Microsoft.Extensions.DependencyInjection;
using ReportEngine.App.ViewModels;
using ReportEngine.App.Views;
using ReportEngine.Shared.Config.Directory;
using ReportEngine.Shared.Config.JsonHelpers;
using ReportEngine.Shared.Config.Logger;
using Serilog;

namespace ReportEngine.App
{
    public class StartUp
    {
        [STAThread]
        public static void Main()
        {
            try
            {
                var connString = JsonHandler.GetConnectionString(DirectoryHelper.GetConfigPath());// Получаем строку подключения из json файла

                Log.Logger = LoggerConfig.InitializeLogger(); // Конфигурация Serilog

                var host = HostFactory.BuildHost(connString); //Конфигурация Host приложения 

                var app = host.Services.GetService<App>(); //Получаем экземпляр приложения 

                var mainWindow = host.Services.GetService<MainWindow>(); //Получаем экземпляр окна

                app.MainWindow = mainWindow;
                mainWindow?.Show(); //Показываем главное окно приложения
                app?.Run(); //Запускаем


                Log.Information("Приложение запущено без ошибок"); //Логируем
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
