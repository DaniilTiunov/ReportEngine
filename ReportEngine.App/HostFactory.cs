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
        public static IHost BuildHost(string connString) //Создание Host приложения
        {
            return Host.CreateDefaultBuilder(). //Конфигурация Host приложения 
                ConfigureServices(services => //Регаем сервисы хуервисы 
                {
                    services.AddDbContext<ReAppContext>(options => //И контекст
                    {
                        options.UseNpgsql(connString); //Используем Npgsql
                    });
                    services.AddSingleton<App>(); //Регистрируем приложение
                    services.AddSingleton<MainWindow>(); //Регистрируем главное окно
                    services.AddScoped<IBaseRepository<User>, UserRepository>(); //Регистрируем репозитории
                    services.AddScoped<ExcelCreator>(); //Регистрируем эксель
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders(); //Очищаем провайдеры логирования
                    logging.AddSerilog(); //Добавляем Serilog
                    logging.SetMinimumLevel(LogLevel.Information); //Устанавливаем уровень логирования
                })
                .Build(); // Возвращаем собранный Host
        }
    }
}
