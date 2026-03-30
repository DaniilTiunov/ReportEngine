using System.Windows.Input;
using ReportEngine.App.Commands;
using ReportEngine.App.Model.CalculationModels;
using ReportEngine.App.Services.Interfaces;
using ReportEngine.Domain.Entities.BaseEntities.Interface;

namespace ReportEngine.App.ViewModels.CalculationSettings;

public class CalculationSettingsViewModel : BaseViewModel
{
    private readonly IDialogService _dialogService;
    private readonly INotificationService _notificationService;

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
            //????
            ApplySelectedItem(StandSettings, selected, propertyName);
            ApplySelectedItem(FrameSettings, selected, propertyName);
        }
    }

    private void ApplySelectedItem(object settings, object selected, string propertyName)
    {
        if (selected == null || settings == null)
            return;

        var selectedEquip = selected as IBaseEquip;

        if (selectedEquip == null)
            return;

        //вытаскиваем свойство с соответствующим именем из настроек
        var nameProp = settings.GetType().GetProperty(propertyName);

        //заполняем это свойство в настройках из переданного объекта (поле Name)
        if (nameProp != null && nameProp.CanWrite)
            nameProp.SetValue(settings, selectedEquip.Name);

        //вытаскиваем свойство с соответствующим именем и постфиксом из настроек
        var entityPropertyName = propertyName + "EntityName";
        var entityNameProp = settings.GetType().GetProperty(entityPropertyName);

        //заполняем это свойство в настройках из переданного объекта (имя типа под интерфейсом)
        if (entityNameProp != null && entityNameProp.CanWrite)
            entityNameProp.SetValue(settings, selectedEquip.GetType().Name);
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
