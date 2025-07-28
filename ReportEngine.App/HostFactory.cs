using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ReportEngine.App.Services;
using ReportEngine.App.ViewModels;
using ReportEngine.App.Views.Controls;
using ReportEngine.App.Views.Windows;
using ReportEngine.Domain.Database.Context;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Entities.Pipes;
using ReportEngine.Domain.Repositories;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Export.ExcelWork;
using Serilog;
using System.Windows.Controls;

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
                    services.AddScoped<ProjectInfoRepository>();
                    services.AddTransient<IGenericBaseRepository<CarbonPipe>, GenericEquipRepository<CarbonPipe>>();

                    // Регистрация сервисов
                    services.AddScoped<ExcelCreator>();
                    services.AddSingleton<GenericEquipWindowFactory>();
                    services.AddSingleton<NavigationService>();
                    services.AddSingleton<IServiceProvider>(provider => provider);
                    // Регистрация ViewModels
                    services.AddScoped<MainWindowViewModel>();
                    services.AddScoped<UsersViewModel>();
                    services.AddScoped<ProjectViewModel>();
                    // Регистрация окон
                    services.AddSingleton(provider =>
                    {
                        var navService = provider.GetRequiredService<NavigationService>();
                        var viewModel = provider.GetRequiredService<MainWindowViewModel>();

                        var mainWindow = new MainWindow(viewModel);
                        var contentHost = mainWindow.FindName("MainContentControl") as ContentControl;
                        navService.InitializeContentHost(contentHost);

                        return mainWindow;
                    });

                    services.AddSingleton<App>();
                    services.AddTransient<UsersView>();
                    services.AddTransient<TreeProjectView>();

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
