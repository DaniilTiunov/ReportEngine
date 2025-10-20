using Microsoft.WindowsAPICodePack.Dialogs;
using ReportEngine.App.AppHelpers;
using ReportEngine.App.Commands;
using ReportEngine.App.Services.Interfaces;
using ReportEngine.Shared.Config.Directory;
using ReportEngine.Shared.Config.IniHeleprs;
using ReportEngine.Shared.Config.JsonHelpers;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace ReportEngine.App.ViewModels;

public class SettingsViewModel : BaseViewModel
{
    private readonly INotificationService _notificationService;
    private string _connectionString;
    private string _savereportPath;

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

    public SettingsViewModel(INotificationService notificationService)
    {
        ApplySettingsCommand = new RelayCommand(ExecuteSaveCommand, _ => true);
        OpenDialog = new RelayCommand(ExecuteOpenDialog, _ => true);

        _notificationService = notificationService;
    }

    public ICommand ApplySettingsCommand { get; set; }
    public ICommand OpenDialog { get; set; }

    public void ExecuteSaveCommand(object p)
    {
        ExceptionHelper.SafeExecute(SaveSettings);
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

        var configPath = DirectoryHelper.GetConfigPath();
        var currentConnectionString = JsonHandler.GetConnectionString(configPath);

        if (ConnectionString != currentConnectionString)
        {
            JsonHandler.SetConnectionString(configPath, ConnectionString);

            Process.Start(new ProcessStartInfo
            {
                FileName = Process.GetCurrentProcess().MainModule.FileName,
                UseShellExecute = true
            });

            _notificationService.ShowInfo("Строка подключения изменена.\nПриложение будет перезапущено");

            Application.Current.Shutdown();
        }
        else
        {
            _notificationService.ShowInfo("Настройки сохранены.");
        }
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