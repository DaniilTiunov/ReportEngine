using System.Diagnostics;
using System.Windows.Input;
using ReportEngine.App.Commands;
using ReportEngine.Shared.Config.Directory;
using ReportEngine.Shared.Config.IniHeleprs;
using ReportEngine.Shared.Config.JsonHelpers;

namespace ReportEngine.App.ViewModels;

public class SettingsViewModel : BaseViewModel
{
    private string _savereportPath;
    private string _connectionString;

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

    public void LoadSettings()
    {
        SaveReportDirPath = SettingsManager.GetReportDirectory();
        ConnectionString = JsonHandler.GetConnectionString(DirectoryHelper.GetConfigPath());
    }
    
    
    public void GetNewDirectory()
    {
 
        
    }
}