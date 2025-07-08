using Microsoft.Extensions.DependencyInjection;
using ReportEngine.App.Config.Directory;
using ReportEngine.App.Config.JsonHelpers;
using ReportEngine.App.Config.Logger;
using ReportEngine.Export.ExcelWork;
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
                var excel = host.Services.GetService<ExcelCreator>(); //Получаем экземпляр экселя
                var mainWindow = host.Services.GetService<MainWindow>(); //Получаем экземпляр главного окна

                mainWindow?.Show(); //Запускаем главное окно
                app?.Run(); //Запускаем

                //excel?.CreateExcelFile().GetAwaiter().GetResult();

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
