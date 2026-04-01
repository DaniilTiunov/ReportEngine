using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using ReportEngine.App.AppHelpers;
using ReportEngine.App.Commands;
using ReportEngine.App.Services.Calculation;
using ReportEngine.App.Services.Interfaces;
using ReportEngine.App.Views.Settings.CalculationParameters.Controls;
using ReportEngine.Domain.Entities.CalculationParameters;
using ReportEngine.Domain.Entities.BaseEntities.Interface;
using ReportEngine.Domain.Entities.CalculationParameters.Enums;


namespace ReportEngine.App.ViewModels;

public class CalculationParametersViewModel : BaseViewModel
{
    private readonly IDialogService _dialogService;
    private readonly IServiceProvider _serviceProvider;
    private readonly ParameterGroupService _parameterGroupService;
    private string? _selectedSetting;
    private object _currentView;
    private CalculationParameter _selectedEquipment = new();

    private ObservableCollection<CalculationParameter> _equipmentsParameters = new();
    private ObservableCollection<CalculationParameter> _standParameters = new();
    private ObservableCollection<CalculationParameter> _frameCost = new();
    private ObservableCollection<CalculationParameter> _humanCost = new();
    private ObservableCollection<CalculationParameter> _sandBlasCost = new();
    private ObservableCollection<CalculationParameter> _electricCost = new();

    public CalculationParametersViewModel(
        IServiceProvider serviceProvider,
        IDialogService dialogService,
        ParameterGroupService parameterGroupService)
    {
        _serviceProvider = serviceProvider;
        _dialogService = dialogService;
        _parameterGroupService = parameterGroupService;

        LoadParametersData();
        InitializeCommands();
    }

    public object CurrentView
    {
        get => _currentView;
        set => Set(ref _currentView, value);
    }

    public string? SelectedSetting
    {
        get => _selectedSetting;
        set
        {
            Set(ref _selectedSetting, value);
            Navigate();
        }
    }

    public CalculationParameter SelectedEquipment
    {
        get => _selectedEquipment;
        set => Set(ref _selectedEquipment, value);
    }

    public ObservableCollection<CalculationParameter> EquipmentsParameters
    {
        get => _equipmentsParameters;
        set => Set(ref _equipmentsParameters, value);
    }

    public ObservableCollection<CalculationParameter> StandsParameters
    {
        get => _standParameters;
        set => Set(ref _standParameters, value);
    }

    public ObservableCollection<CalculationParameter> HumanCost
    {
        get => _humanCost;
        set => Set(ref _humanCost, value);
    }

    public ObservableCollection<CalculationParameter> FrameCost
    {
        get => _frameCost;
        set => Set(ref _frameCost, value);
    }

    public ObservableCollection<CalculationParameter> SandBlastCost
    {
        get => _sandBlasCost;
        set => Set(ref _sandBlasCost, value);
    }

    public ObservableCollection<CalculationParameter> ElectricCost
    {
        get => _electricCost;
        set => Set(ref _electricCost, value);
    }

    public ObservableCollection<string> SettingsItems { get; } = new()
    {
        "Комплектующие",
        "Стенды",
        "Трудозатраты",
        "Рамы",
        "Пескоструй",
        "Электрические"
    };

    public IBaseEquip SelectedEquip { get; set; }
    public ICommand GetSelectedEquipCommand { get; set; }
    public ICommand AddNewEquipParameterCommand { get; set; }
    public ICommand UpdateEquipParameterCommand { get; set; }
    public ICommand DeleteEquipParameterCommand { get; set; }


    private async void LoadParametersData()
    {
        EquipmentsParameters = await _parameterGroupService.GetParametersAsync(CalculationParameterType.Equipments);
        StandsParameters = await _parameterGroupService.GetParametersAsync(CalculationParameterType.StandCost);
        HumanCost = await  _parameterGroupService.GetParametersAsync(CalculationParameterType.HumanCost);
        FrameCost = await _parameterGroupService.GetParametersAsync(CalculationParameterType.FrameCost);
        SandBlastCost = await _parameterGroupService.GetParametersAsync(CalculationParameterType.SandBlastCost);
        ElectricCost = await _parameterGroupService.GetParametersAsync(CalculationParameterType.ElectricCost);
    }

    private void InitializeCommands()
    {
        GetSelectedEquipCommand = new RelayCommand(GetSelectedEquipCommandExecuted, CanAllCommandsExecute);
        AddNewEquipParameterCommand = new RelayCommand(AddNewEquipParameterCommandExecuted, CanAllCommandsExecute);
        UpdateEquipParameterCommand = new RelayCommand(UpdateEquipParameterCommandExecuted, CanAllCommandsExecute);
        DeleteEquipParameterCommand = new RelayCommand(DeleteEquipParameterCommandExecuted, CanAllCommandsExecute);
    }

    public bool CanAllCommandsExecute(object? e)
    {
        return true;
    }

    public void GetSelectedEquipCommandExecuted(object? p)
    {
        SelectedEquip = _dialogService.ShowAllSortamentsDialog();
        SelectedEquipment.Value = SelectedEquip.Name;
        SelectedEquipment.EquipReferenceId = SelectedEquip.Id;

        CollectionRefreshHelper.SafeRefreshCollection(EquipmentsParameters);
    }

    public async void AddNewEquipParameterCommandExecuted(object? p)
    {
        await _parameterGroupService.AddNewParameterAsync(SelectedEquipment, CalculationParameterType.Equipments);
    }

    public async void UpdateEquipParameterCommandExecuted(object? p)
    {
        await _parameterGroupService.UpdateGroupAsync(CalculationParameterType.Equipments, EquipmentsParameters);
    }

    public async void DeleteEquipParameterCommandExecuted(object? p)
    {
        await _parameterGroupService.RemoveParameterFromGroupAsync(SelectedEquipment, CalculationParameterType.Equipments);
        EquipmentsParameters.Remove(SelectedEquipment);
    }

    private void Navigate()
    {
        CurrentView = SelectedSetting switch
        {
            "Комплектующие" => _serviceProvider.GetRequiredService<ComponentsView>(),
            "Стенды" => _serviceProvider.GetRequiredService<StandParametersView>(),
            "Трудозатраты" => _serviceProvider.GetRequiredService<HumanCostView>(),
            "Рамы" => _serviceProvider.GetRequiredService<FrameParametersView>(),
            "Пескоструй" => _serviceProvider.GetRequiredService<SandBlastView>(),
            "Электрические" => _serviceProvider.GetRequiredService<ElectricCostView>(),
            _ => null
        };
    }
}
