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
    private readonly IServiceScopeFactory _scopeFactory;

    public GenericEquipWindowFactory(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public Window CreateWindow<T>(bool isDialog) where T : class, IBaseEquip, new()
    {
        // Создаём scope, который будет жить пока окно открыто
        var scope = _scopeFactory.CreateScope();

        // Разрешаем viewmodel (и все её scoped зависимости) из этого scope
        var repo = scope.ServiceProvider.GetRequiredService<IGenericBaseRepository<T, T>>();
        var viewModel = scope.ServiceProvider.GetRequiredService<GenericEquipViewModel<T>>();

        // Создаём окно и присваиваем DataContext
        var window = new GenericEquipView(isDialog)
        {
            DataContext = viewModel
        };

        // Генерируем колонки (раньше фабрика делала это напрямую)
        GenerateDataGridColumns<T>(window);

        viewModel.OnShowAllEquipCommandExecuted(null);

        window.Closed += (s, e) => scope.Dispose();

        if (isDialog)
        {
            window.GenericEquipDataGrid.IsReadOnly = true;
        }
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

            if (property.Name == "Name")
                column.Width = new DataGridLength(1, DataGridLengthUnitType.SizeToCells);

            if (property == properties[properties.Length - 2])
                column.Width = new DataGridLength(1, DataGridLengthUnitType.Star);

            window.GenericEquipDataGrid.Columns.Add(column);
        }
    }
}
