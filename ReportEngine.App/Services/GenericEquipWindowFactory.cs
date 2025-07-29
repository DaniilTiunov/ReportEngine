using Microsoft.Extensions.DependencyInjection;
using ReportEngine.App.ViewModels;
using ReportEngine.App.Views.Windows;
using ReportEngine.Domain.Entities.BaseEntities;
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

        public Window CreateWindow<TEquip>()
            where TEquip : BaseEquip, new()
        {
            // Получаем репозиторий из DI
            var repository = _serviceProvider.GetRequiredService<IGenericBaseRepository<TEquip>>();

            // Создаем ViewModel
            var viewModel = new GenericEquipViewModel<TEquip>(repository);

            // Создаем окно
            var window = new GenericEquipView();

            // Устанавливаем DataContext
            window.DataContext = viewModel;

            viewModel.OnShowAllEquipCommandExecuted(null);

            return window;
        }
    }
}
