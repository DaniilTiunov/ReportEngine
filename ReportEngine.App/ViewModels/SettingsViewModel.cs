using System.Windows.Input;
using Microsoft.WindowsAPICodePack.Dialogs;
using ReportEngine.App.AppHelpers;
using ReportEngine.App.Commands;
using ReportEngine.App.Services.Interfaces;
using ReportEngine.Shared.Config.Directory;
using ReportEngine.Shared.Config.IniHeleprs;
using ReportEngine.Shared.Config.JsonHelpers;

namespace ReportEngine.App.ViewModels;

public class SettingsViewModel : BaseViewModel
{
    private readonly INotificationService _notificationService;
    private string _connectionString;

    private string _savereportPath;

    public SettingsViewModel(INotificationService notificationService)
    {
        ApplySettingsCommand = new RelayCommand(ExecuteSaveCommand, _ => true);
        OpenDialog = new RelayCommand(ExecuteOpenDialog, _ => true);

        _notificationService = notificationService;
    }

    public string SaveReportDirPath
    {
        get => _savereportPath;
        set => Set(ref _savereportPath, value);
    }

    public string ConnectionString
    {
        get => _connectionString;
        set => Set(ref _connectionString, value);
    }

    public ICommand ApplySettingsCommand { get; set; }
    public ICommand OpenDialog { get; set; }

    public void ExecuteSaveCommand(object p)
    {
        ExceptionHelper.SafeExecute(SaveSettings);
        _notificationService.ShowInfo("Настройки сохранены");
    }

    public void ExecuteOpenDialog(object p)
    {
        ExceptionHelper.SafeExecute(() => { SaveReportDirPath = GetNewDirectory(); });
    }

    public void LoadSettings()
    {
        SaveReportDirPath = SettingsManager.GetReportDirectory();
        ConnectionString = JsonHandler.GetConnectionString(DirectoryHelper.GetConfigPath());
    }

    public void SaveSettings()
    {
        SettingsManager.SetReportDirectory(SaveReportDirPath);
    }

    public string GetNewDirectory()
    {
        var dialog = new CommonOpenFileDialog();
        dialog.IsFolderPicker = true;
        dialog.AddToMostRecentlyUsedList = true;
        dialog.Title = "Выберите папку для сохранения";

        if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            return dialog.FileName;

        return null;
    }
}