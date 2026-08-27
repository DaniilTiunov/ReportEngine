using System.Collections.Concurrent;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DevExpress.XtraRichEdit.Commands;
using ReportEngine.App.Display;
using ReportEngine.App.Model;
using ReportEngine.App.Model.StandsModel;
using ReportEngine.App.Services.Interfaces;
using ReportEngine.App.Services.Notification;
using ReportEngine.Domain.Entities.Armautre;
using ReportEngine.Domain.Entities.BaseEntities;
using ReportEngine.Domain.Entities.BaseEntities.Interface;
using ReportEngine.Domain.Entities.Braces;
using ReportEngine.Domain.Entities.Drainage;
using ReportEngine.Domain.Entities.ElectricComponents;
using ReportEngine.Domain.Entities.ElectricSockets;
using ReportEngine.Domain.Entities.Frame;
using ReportEngine.Domain.Entities.Other;
using ReportEngine.Domain.Entities.Pipes;
using ReportEngine.Domain.Repositories;
using ReportEngine.Domain.Repositories.Interfaces;

namespace ReportEngine.App.ViewModels;

public class AllSortamentsViewModel : BaseViewModel
{
    private readonly GenericRepository _genericRepository;
    private readonly IServiceProvider _serviceProvider;
    private readonly INotificationService _notificationService;
    
    private readonly ConcurrentDictionary<string, Task> _loadingTasks = new();
    private readonly List<string> _comboBoxUnits = new() { "шт", "м", "компл.", "ед." };
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
    private string _currentGroupKey;
    private string _tabItemKey;
    private BaseEquipModel _inputEquip = new();
    private IBaseEquip _selectedEquip;
    private bool _showElectricalFields = false;
    private bool _showContainerFields;
    private bool _showMeasureComboBox;

    public AllSortamentsViewModel(
        IServiceProvider serviceProvider,
        GenericRepository genericRepository, 
        INotificationService notificationService)
    {
        _serviceProvider = serviceProvider;
        _genericRepository = genericRepository;
        _notificationService = notificationService;
        
        AddAsyncCommand = new AsyncRelayCommand(AddNewEquipAsync);
    }

    public AllSortamentsModel CurrentSortamentsModel { get; set; } = new();

    public ICommand AddAsyncCommand { get; set; }
    
    public IBaseEquip SelectedEquip
    {
        get => _selectedEquip;
        set => Set(ref _selectedEquip, value);
    }
    
    public string CurrentGroupKey
    {
        get => _currentGroupKey;
        set
        {
            Set(ref _currentGroupKey, value);
            UpdateAdditionalFieldsVisibility();
        }
    }

    public string TabItemKey
    {
        get => _tabItemKey;
        set => Set(ref _tabItemKey, value);
    }

    public BaseEquipModel InputEquip
    {
        get => _inputEquip;
        set => Set(ref  _inputEquip, value);
    }
    
    public bool ShowElectricalFields
    {
        get => _showElectricalFields;
        set => Set(ref _showElectricalFields, value);
    }

    public bool ShowContainerFields
    {
        get => _showContainerFields;
        set => Set(ref _showContainerFields, value);
    }

    public bool ShowMeasureComboBox
    {
        get => _showMeasureComboBox;
        set
        {
            Set(ref _showMeasureComboBox, value);
        }
    }

    public DataGrid TargetDataGrid { get; set; } = new();

    public Action<IBaseEquip>? SelectionHandler { get; set; }

    private Type GetCurrentEquipType()
    {
        if (string.IsNullOrEmpty(CurrentGroupKey))
            return null;

        _equipTypeMap.TryGetValue(CurrentGroupKey, out var type);
        return type;
    }

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


            DataGridColumn column = new DataGridTextColumn
            {
                Header = GenericEquipMapper.GetColumnName(property.Name),
                Binding = new Binding(property.Name)
            };

            if (property.Name == "Name")
                column.Width = new DataGridLength(1, DataGridLengthUnitType.SizeToCells);

            if (property == properties[properties.Length - 1])
                column.Width = new DataGridLength(1, DataGridLengthUnitType.Star);

            dataGrid.Columns.Add(column);
        }
    }

    private async Task RefreshItems(IBaseEquip newEquip)
    {
        if (CurrentSortamentsModel.EquipGroups.TryGetValue(CurrentGroupKey, out var collection))
        {
            collection.Add(newEquip);
        }
        else
        {
            await LoadGroupAsync(CurrentGroupKey);
        }
    }
    
    private void UpdateAdditionalFieldsVisibility()
    {
        var type = GetCurrentEquipType();
    
        ShowElectricalFields = type != null && 
                               (type.IsSubclassOf(typeof(BaseElectricComponent)) || 
                                type == typeof(BaseElectricComponent));
    
        ShowContainerFields = type != null && type == typeof(Container);
        
        _showMeasureComboBox = !ShowContainerFields;
        OnPropertyChanged(nameof(ShowMeasureComboBox));
    }

    private async Task AddNewEquipAsync()
    {
        var currentType = GetCurrentEquipType();
        var newEquip = (IBaseEquip)Activator.CreateInstance(currentType)!;
        
        newEquip.Name = InputEquip.Name;
        newEquip.Cost = InputEquip.Cost;
        newEquip.ExportDays = InputEquip.ExportDays;
        newEquip.Weight = InputEquip.Weight;
        newEquip.Measure = InputEquip.Measure;
        
        if (newEquip is BaseElectricComponent electrical)
        {
            electrical.CabelInput = InputEquip.CabelInput;
            electrical.Cabel = InputEquip.Cabel;
            electrical.ElectricProtection = InputEquip.ElectricProtection;
        }

        if (newEquip is Container container)
        {
            container.Width = InputEquip.Width;
            container.Height = InputEquip.Height;
            container.Depth = InputEquip.Depth;
        }
        
        await _genericRepository.AddAsync(newEquip);

        GenerateDataGrid(currentType, TargetDataGrid);

        await RefreshItems(newEquip);
        
        _notificationService.ShowInfo("Успешно добавлено");
    }
}