using Mapster;
using ReportEngine.App.ViewModels;
using ReportEngine.Shared.Config.IniHelpers;
using ReportEngine.Shared.Config.IniHelpers.CalculationSettings;
using ReportEngine.Shared.Config.IniHelpers.CalculationSettingsData;

namespace ReportEngine.App.Model.CalculationModels;

public class ElectricalSettingsModel : BaseViewModel
{
    private double _electricalMontage;
    private double _timeMontageCable;
    private double _timeMontageWire;

    public double ElectricalMontage
    {
        get => _electricalMontage;
        set => Set(ref _electricalMontage, value);
    }

    public double TimeMontageWire
    {
        get => _timeMontageWire;
        set => Set(ref _timeMontageWire, value);
    }

    public double TimeMontageCable
    {
        get => _timeMontageCable;
        set => Set(ref _timeMontageCable, value);
    }

    public async Task LoadElectricalDataFromIniAsync()
    {
        var iniData = await CalculationSettingsManager.LoadAsync<ElectricalSettings, ElectricalSettingsData>();

        if (iniData == null)
            return;

        iniData.Adapt(this);
    }

    public async Task SaveElectricalDataToIniAsync()
    {
        var iniData = this.Adapt<ElectricalSettingsData>();
        await CalculationSettingsManager.SaveAsync<ElectricalSettings, ElectricalSettingsData>(iniData);
    }
}