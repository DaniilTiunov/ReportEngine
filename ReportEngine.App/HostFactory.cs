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
using ReportEngine.Domain.Entities.Armautre;
using ReportEngine.Domain.Entities.BaseEntities;
using ReportEngine.Domain.Entities.BaseEntities.Interface;
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
        public static IHost BuildHost(string connString)
        {
            return Host.CreateDefaultBuilder()
                .UseSerilog()
                .ConfigureServices((hostContext, services) =>
                {
                    //Регистрация контекста БД
                    ConfigureDatabase(services, connString);
                    //Регистрация репозиториев
                    ConfigureRepositories(services);
                    //Регистрация сервисов приложения
                    ConfigureApplicationServices(services);
                    //Регистрация ViewModels
                    ConfigureViewModels(services);
                    //Регистрация окон и представлений
                    ConfigureViews(services);

                    services.AddSingleton<App>();
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders()
                          .AddSerilog()
                          .SetMinimumLevel(LogLevel.Information);
                })
                .Build();
        }
        private static void ConfigureDatabase(IServiceCollection services, string connString)
        {
            services.AddDbContext<ReAppContext>(options =>
                options.UseNpgsql(connString));
        }

        private static void ConfigureRepositories(IServiceCollection services)
        {                      
            // Обычные репозитории
            services.AddScoped<IBaseRepository<User>, UserRepository>();
            services.AddScoped<IProjectInfoRepository, ProjectInfoRepository>();
            // Generic-репозитории
            services.AddScoped<IGenericBaseRepository<IBaseEquip, CarbonPipe>, GenericEquipRepository<IBaseEquip, CarbonPipe>>();
            services.AddScoped<IGenericBaseRepository<IBaseEquip, HeaterPipe>, GenericEquipRepository<IBaseEquip, HeaterPipe>>();
            services.AddScoped<IGenericBaseRepository<IBaseEquip, StainlessPipe>, GenericEquipRepository<IBaseEquip, StainlessPipe>>();
            services.AddScoped<IGenericBaseRepository<IBaseEquip, CarbonArmature>, GenericEquipRepository<IBaseEquip, CarbonArmature>>();
            services.AddScoped<IGenericBaseRepository<IBaseEquip, HeaterArmature>, GenericEquipRepository<IBaseEquip, HeaterArmature>>();
            services.AddScoped<IGenericBaseRepository<IBaseEquip, StainlessArmature>, GenericEquipRepository<IBaseEquip, StainlessArmature>>();
        }
        private static void ConfigureApplicationServices(IServiceCollection services)
        {
            services.AddScoped<ExcelCreator>();
            services.AddSingleton<GenericEquipWindowFactory>();
            services.AddSingleton<NavigationService>();
            services.AddSingleton<IServiceProvider>(provider => provider);
        }
        private static void ConfigureViewModels(IServiceCollection services)
        {
            services.AddScoped<MainWindowViewModel>();
            services.AddScoped<UsersViewModel>();
            services.AddScoped<ProjectViewModel>();
            services.AddScoped(typeof(GenericEquipViewModel<IBaseEquip, BaseEquip>));
        }
        private static void ConfigureViews(IServiceCollection services)
        {
            // Главное окно
            services.AddSingleton(provider =>
            {
                var navService = provider.GetRequiredService<NavigationService>();
                var viewModel = provider.GetRequiredService<MainWindowViewModel>();
                var mainWindow = new MainWindow(viewModel);

                navService.InitializeContentHost(mainWindow.FindName("MainContentControl") as ContentControl);
                return mainWindow;
            });
            services.AddTransient<UsersView>();
            services.AddTransient<TreeProjectView>();
            services.AddTransient<GenericEquipView>();
        }
    }
}
