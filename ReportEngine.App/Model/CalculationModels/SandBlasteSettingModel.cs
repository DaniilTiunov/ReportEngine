using Mapster;
using ReportEngine.App.ViewModels;
using ReportEngine.Shared.Config.IniHelpers;
using ReportEngine.Shared.Config.IniHelpers.CalculationSettings;
using ReportEngine.Shared.Config.IniHelpers.CalculationSettingsData;

namespace ReportEngine.App.Model.CalculationModels;

public class SandBlasteSettingModel : BaseViewModel
{
    private double _sandBlastWork;
    private double _timeSandBlastWork;

    public double SandBlastWork
    {
        get => _sandBlastWork; 
        set => Set(ref _sandBlastWork, value);
    }

    public double TimeSandBlastWork
    {
        get =>  _timeSandBlastWork; 
        set => Set(ref _timeSandBlastWork, value);
    }
    
    public async Task LoadSandBlastDataFromIniAsync()
    {
        var iniData = await CalculationSettingsManager.LoadAsync<SandBlastSettings, SandBlasteSettingsData>();

        if (iniData == null)
            return;

        iniData.Adapt(this);
    }

    public async Task SaveSandBlastDataToIniAsync()
    {
        var iniData = this.Adapt<SandBlasteSettingsData>();
        await CalculationSettingsManager.SaveAsync<SandBlastSettings, SandBlasteSettingsData>(iniData);
    }
}