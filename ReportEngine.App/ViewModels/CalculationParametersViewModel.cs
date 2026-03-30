using System.Collections.ObjectModel;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using ReportEngine.App.Commands;
using ReportEngine.App.Services.Calculation;
using ReportEngine.App.Services.Interfaces;
using ReportEngine.App.Views.Settings.CalculationParameters.Controls;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Entities.BaseEntities.Interface;
using ReportEngine.Domain.Entities.CalculationParameters;

namespace ReportEngine.App.ViewModels;

public class CalculationParametersViewModel : BaseViewModel
{
    private readonly IDialogService _dialogService;
    private readonly IServiceProvider _serviceProvider;
    private readonly ParameterGroupService _parameterGroupService;
    private ObservableCollection<CalculationParameter> _equipmentsParameters = new();

    private object _currentView;
    private string? _selectedSetting;
    private ObservableCollection<CalculationParameter> _standParameters = new();
    private CalculationParameter _selectedEquipment;

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

    private async void LoadParametersData()
    {
        EquipmentsParameters = await _parameterGroupService.GetParametersAsync(CalculationParameterType.Equipments);
    }

    private void InitializeCommands()
    {
        GetSelectedEquipCommand = new RelayCommand(GetSelectedEquipCommandExecuted, CanAllCommandsExecute);
    }

    public bool CanAllCommandsExecute(object? e)
    {
        return true;
    }

    public void GetSelectedEquipCommandExecuted(object? p)
    {
        SelectedEquip = _dialogService.ShowAllSortamentsDialog();
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
