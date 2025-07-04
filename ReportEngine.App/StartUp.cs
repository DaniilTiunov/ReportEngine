using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ReportEngine.App.Config.JsonHelpers;
using ReportEngine.Domain.Database.Context;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories;
using ReportEngine.Domain.Repositories.Interfaces;
using Serilog;
using System.IO;

namespace ReportEngine.App
{
    public class StartUp
    {
        [STAThread]
        public static void Main()
        {
            string appDirectory = AppDomain.CurrentDomain.BaseDirectory; //Это приложение путь 

            string configPath = Path.Combine(appDirectory, "Config", "appsettings.json"); //Тянется жысон
            string logPath = Path.Combine(appDirectory, "logs", "log.txt");//Тянется лог

            #if DEBUG //Если сборка Debug, то путь лога будет другой
                   logPath = @"C:\Work\Prjs\ReportEngine.App\ReportEngine.App\logs\log.txt"; //Тянется жысон
            #endif
            
            Directory.CreateDirectory(Path.GetDirectoryName(logPath)); //Создаем каталог если его нет для лога 

            var connString = JsonHandler.GetConnectionString(configPath);

            Log.Logger = new LoggerConfiguration() // Конфигурация Serilog
                .Enrich.FromLogContext()
                .WriteTo.File( //Пишем в файл
                        path: logPath,
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: 7,
                        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                .MinimumLevel.Debug()
                .WriteTo.Console(
                        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
                )
                .CreateLogger();
            try
            {
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
