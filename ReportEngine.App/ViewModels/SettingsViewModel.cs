using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.WindowsAPICodePack.Dialogs;
using Npgsql;
using ReportEngine.App.AppHelpers;
using ReportEngine.App.Commands;
using ReportEngine.App.Services.Interfaces;
using ReportEngine.App.Views;
using ReportEngine.App.Views.Settings.SettingsControls;
using ReportEngine.Shared.Config.Directory;
using ReportEngine.Shared.Config.IniHeleprs;
using ReportEngine.Shared.Config.JsonHelpers;

namespace ReportEngine.App.ViewModels;

public class SettingsViewModel : BaseViewModel
{
    private readonly INotificationService _notificationService;
    private readonly IServiceProvider _serviceProvider;
    private string _connectionString;
    private string _savereportPath;
    private object _currentView;
    private string? _selectedSetting;
    private string _serverAddress;
    private int _serverPort;
    private string _dbName;
    private string _dbUser;
    private string _dbPassword;

    public SettingsViewModel(
        INotificationService notificationService,
        IServiceProvider serviceProvider)
    {
        ApplySettingsCommand = new RelayCommand(ExecuteSaveCommand, _ => true);
        //OpenDialog = new RelayCommand(ExecuteOpenDialog, _ => true);

        LoadSettings();

        _serviceProvider = serviceProvider;
        _notificationService = notificationService;
    }

    public ObservableCollection<string> SettingsItems { get; } = new()
    {
        "Общие",
        "Подключение"
    };

    public string? SelectedSetting
    {
        get => _selectedSetting;
        set
        {
            Set(ref _selectedSetting, value);
            Navigate();
        }
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

    public string ServerAddress
    {
        get => _serverAddress;
        set => Set(ref _serverAddress, value);
    }

    public int ServerPort
    {
        get => _serverPort;
        set => Set(ref _serverPort, value);
    }

    public string DbName
    {
        get => _dbName;
        set => Set(ref _dbName, value);
    }

    public string DbUser
    {
        get => _dbUser;
        set => Set(ref _dbUser, value);
    }

    public string DbPassword
    {
        get => _dbPassword;
        set => Set(ref _dbPassword, value);
    }

    public object CurrentView
    {
        get => _currentView;
        set => Set(ref _currentView, value);
    }

    public ICommand ApplySettingsCommand { get; set; }
    public ICommand OpenDialog { get; set; }

    public void ExecuteSaveCommand(object p)
    {
        ExceptionHelper.SafeExecute(SaveSettings);
    }

    //public void ExecuteOpenDialog(object p)
    //{
    //    ExceptionHelper.SafeExecute(() => { SaveReportDirPath = GetNewDirectory(); });
    //}

    private void Navigate()
    {
        CurrentView = SelectedSetting switch
        {
            "Общие" => _serviceProvider.GetRequiredService<CommonSettings>(),
            "Подключение" => _serviceProvider.GetRequiredService<ConnectionSettings>(),
            _ => null
        };
    }

    public void LoadSettings()
    {
        SaveReportDirPath = SettingsManager.GetReportDirectory();
        ConnectionString = JsonHandler.GetConnectionString(DirectoryHelper.GetConfigPath());
        ConnectionStringParse(ConnectionString);
    }

    public void ConnectionStringParse(string connectionString)
    {
        var builder = new NpgsqlConnectionStringBuilder(connectionString);

        ServerAddress = builder.Host;
        DbName = builder.Database;
        ServerPort = builder.Port;
        DbUser = builder.Username;
        DbPassword = builder.Password;
    }

    public string BuildConnectionString(string host,
                                        int port,
                                        string database,
                                        string username,
                                        string password)
    {
        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = host,
            Port = port,
            Database = database,
            Username = username,
            Password = password
        };

        return builder.ConnectionString;
    }

    public void SaveSettings()
    {
        SettingsManager.SetReportDirectory(SaveReportDirPath);

        var configPath = DirectoryHelper.GetConfigPath();
        var currentConnectionString = JsonHandler.GetConnectionString(configPath);

        var newConnectionString =
            BuildConnectionString(ServerAddress, ServerPort, DbName, DbUser, DbPassword);

        if (newConnectionString != currentConnectionString)
        {
            var result = _notificationService
                .ShowConfirmation("Строка подключения изменена.\nПриложение будет перезапущено. Продолжить?");

            if (!result)
                return;

            JsonHandler.SetConnectionString(configPath, newConnectionString);

            Process.Start(new ProcessStartInfo
            {
                FileName = Process.GetCurrentProcess().MainModule!.FileName!,
                UseShellExecute = true
            });

            Application.Current.Shutdown();
        }
        else
        {
            _notificationService.ShowInfo("Настройки сохранены.");
        }
    }
}
