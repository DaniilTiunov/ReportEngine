using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ReportEngine.App.Config.Directory;
using ReportEngine.App.Config.JsonHelpers;
using ReportEngine.App.Config.Logger;
using ReportEngine.Domain.Database.Context;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories;
using ReportEngine.Domain.Repositories.Interfaces;
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
                var connString = JsonHandler.GetConnectionString(DirectoryHelper.GetConfigPath());

                Log.Logger = LoggerConfig.InitializeLogger(); // Конфигурация Serilog

                var host = Host.CreateDefaultBuilder(). //Конфигурация Host приложения
                ConfigureServices(services => //Регаем сервисы хуервисы
                {
                    services.AddDbContext<ReAppContext>(options => //И контекст
                    {
                        options.UseNpgsql(connString);
                    });
                    services.AddSingleton<App>();
                    services.AddScoped<IBaseRepository<User>, UserRepository>();
                    services.AddSingleton<MainWindow>();
                    services.AddScoped<ExcelCreater>();
                })
                .ConfigureLogging(logging =>
                {
                    logging.SetMinimumLevel(LogLevel.Information).ClearProviders();
                    logging.AddSerilog();
                })
                .Build();

                var app = host.Services.GetService<App>(); //Получаем экземпляр приложения
                var mainWindow = host.Services.GetService<MainWindow>(); //Получаем экземпляр главного окна
            
                mainWindow?.Show(); //Запускаем главное окно
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
