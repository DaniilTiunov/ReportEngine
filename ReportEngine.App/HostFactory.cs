using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ReportEngine.App.Services;
using ReportEngine.App.ViewModels;
using ReportEngine.App.Views.Windows;
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
            return Host.CreateDefaultBuilder().
                UseSerilog().//Конфигурация Host приложения 
                ConfigureServices(services => //Регаем сервисы хуервисы 
                {
                    services.AddDbContext<ReAppContext>(options => //И контекст
                    {
                        options.UseNpgsql(connString); //Используем Npgsql
                    });
                    //Регистрируем репозитории
                    services.AddScoped<IBaseRepository<User>, UserRepository>();
                    services.AddScoped<IBaseRepository<ProjectInfo>, ProjectInfoRepository>();

                    // Регистрация сервисов
                    services.AddScoped<ExcelCreator>();
                    services.AddSingleton<NavigationService>(); //Регистрация сервиса навигации>
                    // Регистрация ViewModels
                    services.AddScoped<MainWindowViewModel>();
                    services.AddScoped<UsersViewModel>();

                    // Регистрация окон
                    services.AddSingleton<App>();
                    services.AddSingleton(provider =>
                    {
                        var viewModel = provider.GetRequiredService<MainWindowViewModel>();

                        return new MainWindow(viewModel);
                    });
                    services.AddSingleton<UsersView>();
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
