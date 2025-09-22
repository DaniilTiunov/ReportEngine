using System.Windows.Input;
using ReportEngine.App.Commands;
using ReportEngine.App.Model.CalculationModels;
using ReportEngine.App.Services.Interfaces;

namespace ReportEngine.App.ViewModels.CalculationSettings;

public class CalculationSettingsViewModel : BaseViewModel
{
    private readonly INotificationService _notificationService;


    public CalculationSettingsViewModel(INotificationService notificationService)
    {
        InitializeCommands();
        _notificationService = notificationService;
    }

    public HumanCostSettingsModel HumanCosts { get; set; } = new();
    public StandSettingsModel StandSettings { get; set; } = new();

    public ICommand SaveSettingsCommand { get; set; }

    private void InitializeCommands()
    {
        SaveSettingsCommand = new RelayCommand(OnSaveSettingsCommandExecuted, _ => true);
    }

    public async void OnSaveSettingsCommandExecuted(object p)
    {
        await SaveSettings();
    }

    public async Task LoadSettingsAsync()
    {
        await HumanCosts.LoadHumanCostDataFromIniAsync();
        await StandSettings.LoadStandsSettingsDataAsync();
    }

    public async Task SaveSettings()
    {
        await HumanCosts.SaveDataToIniAsync();
        await StandSettings.SaveDataToIniAsync();

        _notificationService.ShowInfo("Настройки сохранены.");
    }
}