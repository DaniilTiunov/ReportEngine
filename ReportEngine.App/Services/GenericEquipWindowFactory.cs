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
        /// <typeparam name="TEquip">Тип оборудования, для которого создается окно.</typeparam>
        /// <returns>Созданное окно с настроенным DataContext и сгенерированными столбцами.</returns>
        public Window CreateWindow<T, TEquip>()
            where T : class, IBaseEquip, new() // Ограничение: T должен реализовывать интерфейс IBaseEquip
            //where TEquip : class, new() // Ограничение: TEquip должен быть классом и иметь публичный конструктор без параметров
        {
            // Получаем репозиторий из DI
            var repository = _serviceProvider.GetRequiredService<IGenericBaseRepository<T, T>>();
            // Создаем ViewModel
            var viewModel = new GenericEquipViewModel<T, T>(repository);
            // Создаем окно
            var window = new GenericEquipView();
            // Устанавливаем DataContext окна на созданную ViewModel
            // Это позволяет окну использовать ViewModel для привязки данных
            window.DataContext = viewModel;
            // Генерируем столбцы для DataGrid в окне на основе свойств типа TEquip
            GenerateDataGridColumns(viewModel, window);
            // Выполняем команду для отображения всего оборудования
            viewModel.OnShowAllEquipCommandExecuted(null);
            // Возвращаем созданное и настроенное окно
            return window;
        }
        /// <summary>
        /// Генерирует столбцы для DataGrid на основе свойств обобщенного типа TEquip.
        /// </summary>
        /// <typeparam name="T">Тип, реализующий интерфейс IBaseEquip.</typeparam>
        /// <typeparam name="TEquip">Тип оборудования, для которого генерируются столбцы.</typeparam>
        /// <param name="viewModel">ViewModel, содержащая данные для отображения.</param>
        /// <param name="window">Окно, содержащее DataGrid, для которого генерируются столбцы.</param>
        private void GenerateDataGridColumns<T>(GenericEquipViewModel<T, T> viewModel, GenericEquipView window)
            where T : class, IBaseEquip, new() // Ограничение: T должен реализовывать интерфейс IBaseEquip
            //where TEquip : class, new() // Ограничение: TEquip должен быть классом и иметь публичный конструктор без параметров
        {
            // Очистка существующих столбцов в DataGrid
            window.GenericEquipDataGrid.Columns.Clear();
            // Получение типа TEquip
            var itemType = typeof(T);
            // Получение всех свойств типа TEquip
            var properties = itemType.GetProperties();
            // Перебор всех свойств
            foreach (var property in properties)
            {
                // Пропускаем свойство с именем "Id", так как оно не нужно для отображения
                if (property.Name == "Id")
                    continue;
                // Создание нового столбца для DataGrid
                DataGridColumn column;
                // Создание текстового столбца
                column = new DataGridTextColumn
                {
                    // Установка заголовка столбца на основе имени свойства
                    Header = GenericEquipMapper.GetColumnName(property.Name),
                    // Привязка столбца к свойству объекта
                    Binding = new Binding(property.Name)
                };
                // Добавление созданного столбца в DataGrid
                window.GenericEquipDataGrid.Columns.Add(column);
            }
        }
    }
}