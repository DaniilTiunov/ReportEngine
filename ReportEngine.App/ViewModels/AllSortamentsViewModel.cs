using ReportEngine.App.Display;
using ReportEngine.App.Model;
using ReportEngine.Domain.Entities.Armautre;
using ReportEngine.Domain.Entities.BaseEntities.Interface;
using ReportEngine.Domain.Entities.Braces;
using ReportEngine.Domain.Entities.Drainage;
using ReportEngine.Domain.Entities.ElectricComponents;
using ReportEngine.Domain.Entities.ElectricSockets;
using ReportEngine.Domain.Entities.Frame;
using ReportEngine.Domain.Entities.Other;
using ReportEngine.Domain.Entities.Pipes;
using ReportEngine.Domain.Repositories.Interfaces;
using System.Collections.Concurrent;
using System.Windows.Controls;
using System.Windows.Data;

namespace ReportEngine.App.ViewModels;

public class AllSortamentsViewModel : BaseViewModel
{
    private readonly Dictionary<string, Type> _equipTypeMap = new()
    {
        { "Трубы\\Жаропрочные", typeof(HeaterPipe) },
        { "Трубы\\Нержавеющие", typeof(StainlessPipe) },
        { "Трубы\\Углеродистые", typeof(CarbonPipe) },
        { "Арматуры\\Жаропрочные", typeof(HeaterArmature) },
        { "Арматуры\\Нержавеющие", typeof(StainlessArmature) },
        { "Арматуры\\Углеродистые", typeof(CarbonArmature) },
        { "Тройники и КМЧ\\Жаропрочные", typeof(HeaterSocket) },
        { "Тройники и КМЧ\\Нержавеющие", typeof(StainlessSocket) },
        { "Тройники и КМЧ\\Углеродистые", typeof(CarbonSocket) },
        { "Дренажи и крепления", typeof(Drainage) },
        { "Детали рамы", typeof(FrameDetail) },
        { "Комплектующие для стойки", typeof(PillarEqiup) },
        { "Прокат", typeof(FrameRoll) },
        { "Крепление датчиков", typeof(SensorBrace) },
        { "Крепление дренажа", typeof(DrainageBrace) },
        { "Крепление клеммных коробок", typeof(BoxesBrace) },
        { "Кабельная продукция", typeof(CabelProduction) },
        { "Кабельные вводы", typeof(CabelInput) },
        { "Клеммные коробки", typeof(CabelBoxe) },
        { "Обогрев", typeof(Heater) },
        { "Средства прокладки", typeof(CabelProtection) },
        { "Прочие", typeof(Other) },
        { "Тара", typeof(Container) }
    };

    private readonly IServiceProvider _serviceProvider;

    private IBaseEquip _selectedEquip;

    // Словарь текущих задач загрузки по ключу группы — предотвращает параллельный доступ
    private readonly ConcurrentDictionary<string, Task> _loadingTasks = new();

    public AllSortamentsViewModel(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public AllSortamentsModel CurrentSortamentsModel { get; set; } = new();

    public IBaseEquip SelectedEquip
    {
        get => _selectedEquip;
        set => Set(ref _selectedEquip, value);
    }

    public Action<IBaseEquip>? SelectionHandler { get; set; }

    public async Task LoadGroupAsync(string groupKey)
    {
        if (!_equipTypeMap.TryGetValue(groupKey, out var type))
            return;

        if (CurrentSortamentsModel.EquipGroups.ContainsKey(groupKey))
            return;

        var loadingTask = _loadingTasks.GetOrAdd(groupKey, _ => LoadGroupInternalAsync(groupKey, type));

        try
        {
            await loadingTask;
        }
        finally
        {
            _loadingTasks.TryRemove(groupKey, out _);
        }
    }

    private async Task LoadGroupInternalAsync(string groupKey, Type type)
    {
        var repoType = typeof(IGenericBaseRepository<,>).MakeGenericType(type, type);
        var repository = _serviceProvider.GetService(repoType);
        if (repository == null)
            return;

        var items = await ((dynamic)repository).GetAllAsync();
        CurrentSortamentsModel.SetEquipGroup(groupKey, items);
    }

    public void GenerateDataGridByTag(DataGrid grid, string groupKey)
    {
        if (!_equipTypeMap.TryGetValue(groupKey, out var type))
            return;

        GenerateDataGrid(type, grid);
    }

    private void GenerateDataGrid(Type equipType, DataGrid dataGrid)
    {
        dataGrid.Columns.Clear();
        var properties = equipType.GetProperties()
            .OrderByDescending(x => x.Name == "Name").ToArray();

        foreach (var property in properties)
        {
            if (property.Name == "Id")
                continue;

            var column = new DataGridTextColumn
            {
                Header = GenericEquipMapper.GetColumnName(property.Name),
                Binding = new Binding(property.Name),
            };

            if (property.Name == "Name")
                column.Width = new DataGridLength(1, DataGridLengthUnitType.SizeToCells);


            if (property == properties[properties.Length - 1])
                column.Width = new DataGridLength(1, DataGridLengthUnitType.Star);

            dataGrid.Columns.Add(column);
        }
    }
}
