using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ReportEngine.Domain.Database.Context;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Export.ExcelWork;
using Serilog;

namespace ReportEngine.App
{
    public class HostFactory
    {
        public static IHost BuildHost(string connString)
        {
            return Host.CreateDefaultBuilder(). //Конфигурация Host приложения
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
                    logging.ClearProviders(); //Очищаем провайдеры логирования
                    logging.AddSerilog();
                    logging.SetMinimumLevel(LogLevel.Information);
                })
                .Build();
        }
    }
}
