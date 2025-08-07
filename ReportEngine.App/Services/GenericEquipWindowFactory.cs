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
    /// <summary>
    /// Фабрика для создания окон, отображающих типовое оборудование.
    /// </summary>
    public class GenericEquipWindowFactory
    {
        private readonly IServiceProvider _serviceProvider;
        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="GenericEquipWindowFactory"/>.
        /// </summary>
        /// <param name="serviceProvider">Провайдер сервисов для получения необходимых зависимостей.</param>
        public GenericEquipWindowFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        /// <summary>
        /// Создает и возвращает новое окно для отображения оборудования.
        /// </summary>
        /// <typeparam name="T">Тип, реализующий интерфейс IBaseEquip.</typeparam>
        /// <returns>Созданное окно с настроенным DataContext и сгенерированными столбцами.</returns>
        public Window CreateWindow<T>() where T : class, IBaseEquip, new()
        {
            // Получаем репозиторий из DI
            var repository = _serviceProvider.GetRequiredService<IGenericBaseRepository<T, T>>();
            // Создаем ViewModel
            var viewModel = new GenericEquipViewModel<T>(repository);
            // Создаем окно
            var window = new GenericEquipView();
            // Устанавливаем DataContext окна на созданную ViewModel
            // Это позволяет окну использовать ViewModel для привязки данных
            window.DataContext = viewModel;
            // Генерируем столбцы для DataGrid в окне на основе свойств типа TEquip
            GenerateDataGridColumns<T>(window);
            // Выполняем команду для отображения всего оборудования
            viewModel.OnShowAllEquipCommandExecuted(null);
            // Возвращаем созданное и настроенное окно
            return window;
        }
        /// <summary>
        /// Генерирует столбцы для DataGrid на основе свойств обобщенного типа TEquip.
        /// </summary>
        /// <typeparam name="T">Тип, реализующий интерфейс IBaseEquip.</typeparam>
        /// <param name="viewModel">ViewModel, содержащая данные для отображения.</param>
        /// <param name="window">Окно, содержащее DataGrid, для которого генерируются столбцы.</param>
        private void GenerateDataGridColumns<T>(GenericEquipView window)
            where T : class, IBaseEquip, new()
        {
            window.GenericEquipDataGrid.Columns.Clear();
            var properties = typeof(T).GetProperties();
            foreach (var property in properties)
            {
                if (property.Name == "Id")
                    continue;
                if (property.Name == "FormedFrameId")
                    continue;
                if (property.Name == "FormedFrame")
                    continue;


                DataGridColumn column = new DataGridTextColumn
                {
                    Header = GenericEquipMapper.GetColumnName(property.Name),
                    Binding = new Binding(property.Name)                 
                };

                if (property == properties[properties.Length - 1]) // Растягивание последнего столбца, чтобы избежать появление
                    column.Width = new DataGridLength(1, DataGridLengthUnitType.Star); // Пустого столбца

                window.GenericEquipDataGrid.Columns.Add(column);
            }
        }
    }
}