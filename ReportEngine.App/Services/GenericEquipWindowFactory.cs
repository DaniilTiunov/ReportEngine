using Microsoft.Extensions.DependencyInjection;
using ReportEngine.App.ViewModels;
using ReportEngine.App.Views.Windows;
using ReportEngine.Domain.Entities.BaseEntities.Interface;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Shared.Config.DebugConsol;
using System.Diagnostics;
using System.Windows;

namespace ReportEngine.App.Services
{
    public class GenericEquipWindowFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public GenericEquipWindowFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Window CreateWindow<T, TEquip>()
            where T : IBaseEquip
            where TEquip : class, new()
        {
            // Получаем репозиторий из DI
            var repository = _serviceProvider.GetRequiredService<IGenericBaseRepository<T, TEquip>>();

            // Создаем ViewModel
            var viewModel = new GenericEquipViewModel<T, TEquip>(repository);

            // Создаем окно
            var window = new GenericEquipView();

            // Устанавливаем DataContext
            window.DataContext = viewModel;

            viewModel.OnShowAllEquipCommandExecuted(null);

            return window;
        }
    }
}
