using Microsoft.Extensions.DependencyInjection;
using ReportEngine.App.AppHelpers;
using ReportEngine.App.Display;
using ReportEngine.App.Model;
using ReportEngine.Domain.Entities.BaseEntities.Interface;
using ReportEngine.Domain.Entities.Pipes;
using ReportEngine.Domain.Repositories.Interfaces;
using System.Windows.Controls;
using System.Windows.Data;

namespace ReportEngine.App.ViewModels;

public class AllSortamentsViewModel : BaseViewModel
{
    private readonly IServiceProvider _serviceProvider;

    private readonly Dictionary<string, Type> _equipTypeMap = new()
    {
        { "Трубы\\Жаропрочные", typeof(HeaterPipe) },
        { "Трубы\\Нержавеющие", typeof(StainlessPipe) },
        { "Трубы\\Углеродистые", typeof(CarbonPipe) },
    };

    public AllSortamentsModel CurrentSortamentsModel { get; set; } = new();

    public AllSortamentsViewModel(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task LoadDataAsync()
    {
        await ExceptionHelper.SafeExecuteAsync(async () =>
        {
            foreach (var entry in _equipTypeMap)
            {
                string groupName = entry.Key;
                Type type = entry.Value;

                Type repoType = typeof(IGenericBaseRepository<,>).MakeGenericType(type, type);

                var repoEquip = _serviceProvider.GetRequiredService(repoType);
                if (repoEquip == null)
                    continue;

                // Вызываем GetAllAsync() через reflection
                var method = repoType.GetMethod("GetAllAsync");
                var task = (Task)method.Invoke(repoEquip, null);
                await task.ConfigureAwait(false);

                // Получаем результат
                var resultProperty = task.GetType().GetProperty("Result");
                var items = resultProperty?.GetValue(task) as IEnumerable<IBaseEquip>;
                if (items != null)
                {
                    CurrentSortamentsModel.SetEquipGroup(groupName, items);
                }
            }
        });       
    }

    private void GenerateDataGrid<T>(DataGrid dataGrid)
        where T : class, IBaseEquip
    {
        dataGrid.Columns.Clear();
        var properties = typeof(T).GetProperties();
        foreach (var property in properties)
        {
            if (property.Name == "Id")
                continue;

            DataGridColumn column = new DataGridTextColumn
            {
                Header = GenericEquipMapper.GetColumnName(property.Name),
                Binding = new Binding(property.Name)
            };

            if (property ==
                properties[properties.Length - 1])
                column.Width = new DataGridLength(1, DataGridLengthUnitType.Star);
        }
    }
}