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

namespace ReportEngine.App
{
    public class StartUp
    {
        [STAThread]
        public static void Main()
        {
            JsonHandler jsonHandler = new JsonHandler(@"C:\Work\Prjs\ReportEngine.App\ReportEngine.App\Config\appsettings.json"); // Тут разобраться почему только полная строка
            var connString = jsonHandler.GetConnectionString(); // Из жысона получаем строку

            Log.Logger = new LoggerConfiguration() // Конфигурация Serilog
                .Enrich.FromLogContext()
                .WriteTo.File( //Пишем в файл
                        path: @"C:\Work\Prjs\ReportEngine.App\ReportEngine.App\logs\log.txt",
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
