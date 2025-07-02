using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ReportEngine.App.Config.JsonHelpers;
using ReportEngine.Domain.Database.Context;
using Serilog;

namespace ReportEngine.App
{
    public class StartUp
    {
        [STAThread]
        public static void Main()
        {
            JsonHandler jsonHandler = new JsonHandler(@"C:\Work\Prjs\ReportEngine.App\ReportEngine.App\Config\appsettings.json");
            var connString = jsonHandler.GetConnectionString();

            Log.Logger = new LoggerConfiguration() //Конфигурация Serilog
                .Enrich.FromLogContext()
                .WriteTo.File(
                        path: @"C:\Work\Prjs\ReportEngine.App\ReportEngine.App\logs\log.txt",
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit: 7,
                        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                .MinimumLevel.Debug()
                .WriteTo.Console(
                        outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
                )
                .CreateLogger();

            var host = Host.CreateDefaultBuilder(). //Конфигурация Host приложения
                ConfigureServices(services => //Регаем сервисы
                {
                    services.AddDbContext<ReAppContext>(options => //И контекст
                    {
                        options.UseNpgsql(connString);
                    });
                    services.AddSingleton<App>();
                    services.AddSingleton<MainWindow>();
                })

                .ConfigureLogging(logging =>
                {
                    logging.SetMinimumLevel(LogLevel.Information).ClearProviders();
                    logging.AddSerilog();
                })
                .Build();

            var app = host.Services.GetService<App>(); //Получаем экземпляр приложения

            app?.Run(); //Запускаем

            Log.Information("Приложение запущено без ошибок"); //Логируем



        }
    }
}
