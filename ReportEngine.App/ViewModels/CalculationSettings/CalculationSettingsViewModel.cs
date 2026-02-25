using System.Diagnostics;
using System.Windows.Input;
using ReportEngine.App.Commands;
using ReportEngine.App.Model.CalculationModels;
using ReportEngine.App.Services.Interfaces;

namespace ReportEngine.App.ViewModels.CalculationSettings;

public class CalculationSettingsViewModel : BaseViewModel
{
    private readonly INotificationService _notificationService;
    private readonly IDialogService _dialogService;

    public CalculationSettingsViewModel(
        INotificationService notificationService,
        IDialogService dialogService)
    {
        InitializeCommands();
        _notificationService = notificationService;
        _dialogService = dialogService;
    }

    public HumanCostSettingsModel HumanCosts { get; set; } = new();
    public StandSettingsModel StandSettings { get; set; } = new();
    public FrameSettingsModel FrameSettings { get; set; } = new();
    public SandBlasteSettingModel SandBlaste { get; set; } = new();
    public ElectricalSettingsModel ElectricalSettings { get; set; } = new();

    public ICommand SaveSettingsCommand { get; set; }
    public ICommand ShowAllEqipDialogCommand { get; set; }

    private void InitializeCommands()
    {
        SaveSettingsCommand = new RelayCommand(OnSaveSettingsCommandExecuted, _ => true);
        ShowAllEqipDialogCommand = new RelayCommand(OnShowAllEqipDialogCommandExecuted, _ => true);
    }

    public void OnShowAllEqipDialogCommandExecuted(object p)
    {
        var selected = _dialogService.ShowAllSortamentsDialog();

        if (selected == null)
            return;

        if (p is string propertyName && !string.IsNullOrWhiteSpace(propertyName))
        {
            ApplySelectedItem(StandSettings, selected, propertyName);
            ApplySelectedItem(FrameSettings, selected, propertyName);
        }
    }

    private void ApplySelectedItem(object settings, object selected, string propertyName)
    {
        if (selected == null || settings == null)
            return;

        var type = settings.GetType();

        var nameProp = type.GetProperty(propertyName);
        if (nameProp != null && nameProp.CanWrite)
            nameProp.SetValue(settings, selected.GetType().GetProperty("Name")?.GetValue(selected));

        var measurePropertyName = propertyName + "Measure";
        var measureProp = type.GetProperty(measurePropertyName);

        if (measureProp != null && measureProp.CanWrite)
            measureProp.SetValue(settings, selected.GetType().GetProperty("Measure")?.GetValue(selected));
    }

    public async void OnSaveSettingsCommandExecuted(object p)
    {
        await SaveSettings();
    }

    public async Task LoadSettingsAsync()
    {
        await HumanCosts.LoadHumanCostDataFromIniAsync();
        await StandSettings.LoadStandsSettingsDataAsync();
        await FrameSettings.LoadFrameDataFromIniAsync();
        await SandBlaste.LoadSandBlastDataFromIniAsync();
        await ElectricalSettings.LoadElectricalDataFromIniAsync();
    }

    public async Task SaveSettings()
    {
        await HumanCosts.SaveDataToIniAsync();
        await StandSettings.SaveDataToIniAsync();
        await FrameSettings.SaveFrameDataToIniAsync();
        await SandBlaste.SaveSandBlastDataToIniAsync();
        await ElectricalSettings.SaveElectricalDataToIniAsync();

        _notificationService.ShowInfo("Настройки сохранены.");
    }
}
