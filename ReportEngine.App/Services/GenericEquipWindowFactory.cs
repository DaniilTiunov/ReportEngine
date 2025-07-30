using Microsoft.Extensions.DependencyInjection;
using ReportEngine.App.Display;
using ReportEngine.App.ViewModels;
using ReportEngine.App.Views.Windows;
using ReportEngine.Domain.Entities.BaseEntities.Interface;
using ReportEngine.Domain.Repositories.Interfaces;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

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

            // Генерируем колонки
            GenerateDataGridColumns(viewModel, window);

            viewModel.OnShowAllEquipCommandExecuted(null);

            return window;
        }
        private void GenerateDataGridColumns<T, TEquip>(GenericEquipViewModel<T, TEquip> viewModel, GenericEquipView window)
            where T : IBaseEquip
            where TEquip : class, new()
        {
            window.GenericEquipDataGrid.Columns.Clear();

            var itemType = typeof(TEquip);
            var properties = itemType.GetProperties();

            foreach (var property in properties)
            {
                DataGridColumn column;

                if (property.Name == "Id")
                    continue;

                column = new DataGridTextColumn
                {
                    Header = GenericEquipMapper.GetColumnName(property.Name),
                    Binding = new Binding(property.Name)
                };


                window.GenericEquipDataGrid.Columns.Add(column);
            }
        }
    }
}
