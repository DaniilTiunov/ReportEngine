using Microsoft.Extensions.DependencyInjection;
using ReportEngine.App.Display;
using ReportEngine.App.ViewModels;
using ReportEngine.App.Views.Windows;
using ReportEngine.Domain.Entities.BaseEntities.Interface;
using ReportEngine.Domain.Repositories.Interfaces;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ReportEngine.App.Services;

/// <summary>
///     Фабрика для создания окон, отображающих типовое оборудование.
/// </summary>
public class GenericEquipWindowFactory
{
    private readonly IServiceProvider _serviceProvider;

    public GenericEquipWindowFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public Window CreateWindow<T>(bool isDialog) where T : class, IBaseEquip, new()
    {
        // Получаем репозиторий из DI
        var repository = _serviceProvider.GetRequiredService<IGenericBaseRepository<T, T>>();
        // Создаем ViewModel
        var viewModel = new GenericEquipViewModel<T>(repository);
        // Создаем окно
        var window = new GenericEquipView(isDialog);
        // Устанавливаем DataContext окна на созданную ViewModel
        // Это позволяет окну использовать ViewModel для привязки данных
        window.DataContext = viewModel;
        // Генерируем столбцы для DataGrid в окне на основе свойств типа TEquip
        GenerateDataGridColumns<T>(window);
        // Выполняем команду для отображения всего оборудования
        viewModel.OnShowAllEquipCommandExecuted(null);

        if (isDialog)
        {
            window.GenericEquipDataGrid.IsReadOnly = true;
        }
        // Возвращаем созданное и настроенное окно
        return window;
    }

    private void GenerateDataGridColumns<T>(GenericEquipView window)
        where T : class, IBaseEquip, new()
    {
        window.GenericEquipDataGrid.Columns.Clear();

        var properties = typeof(T).GetProperties()
            .OrderByDescending(x => x.Name == "Name").ToArray();

        foreach (var property in properties)
        {
            if (property.Name == "Id")
                continue;

            DataGridColumn column = new DataGridTextColumn
            {
                Header = GenericEquipMapper.GetColumnName(property.Name),
                Binding = new Binding(property.Name)
            };

            if (property == properties[properties.Length - 2])
                column.Width = new DataGridLength(1, DataGridLengthUnitType.Star);

            window.GenericEquipDataGrid.Columns.Add(column);
        }
    }
}
