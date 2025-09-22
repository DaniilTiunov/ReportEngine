using Mapster;
using ReportEngine.App.ViewModels;
using ReportEngine.Shared.Config.IniHelpers;
using ReportEngine.Shared.Config.IniHelpers.CalculationSettings;
using ReportEngine.Shared.Config.IniHelpers.CalculationSettingsData;

namespace ReportEngine.App.Model.CalculationModels;

public class FrameSettingsModel : BaseViewModel
{
    private double _frameProduction;
    private double _timeForProductionFrame;
    private double _timeForPrepareFrame;
    private double _painting;
    private double _timeForPaintFrame;
    private double _timeForPaintObv;
    private string _materialOne;
    private string _materialTwo;
    private double _countMaterialOne;
    private double _countMaterialTwo;
    private double _assemblyCost;
    private double _timeForAssemblyWork;
    private double _pillarCost;
    private double _timePillarProduction;
    private double _closetWorkCost;
    private double _timeClosetPrepare;

    public double FrameProduction
    {
        get => _frameProduction;
        set => Set(ref _frameProduction, value);
    } // Изготовление рам

    public double TimeForProductionFrame
    {
        get => _timeForProductionFrame;
        set => Set(ref _timeForProductionFrame, value);
    }

    public double TimeForPrepareFrame
    {
        get => _timeForPrepareFrame;
        set => Set(ref _timeForPrepareFrame, value);
    }

    public double Painting
    {
        get => _painting;
        set => Set(ref _painting, value);
    }

    public double TimeForPaintFrame
    {
        get => _timeForPaintFrame;
        set => Set(ref _timeForPaintFrame, value);
    }

    public double TimeForPaintObv
    {
        get => _timeForPaintObv;
        set => Set(ref _timeForPaintObv, value);
    }

    public string MaterialOne
    {
        get => _materialOne;
        set => Set(ref _materialOne, value);
    }

    public string MaterialTwo
    {
        get => _materialTwo;
        set => Set(ref _materialTwo, value);
    }

    public double CountMaterialOne
    {
        get => _countMaterialOne;
        set => Set(ref _countMaterialOne, value);
    }

    public double CountMaterialTwo
    {
        get => _countMaterialTwo;
        set => Set(ref _countMaterialTwo, value);
    }

    public double AssemblyCost
    {
        get => _assemblyCost;
        set => Set(ref _assemblyCost, value);
    }

    public double TimeForAssemblyWork
    {
        get => _timeForAssemblyWork;
        set => Set(ref _timeForAssemblyWork, value);
    }

    public double PillarCost
    {
        get => _pillarCost;
        set => Set(ref _pillarCost, value);
    }

    public double TimePillarProduction
    {
        get => _timePillarProduction;
        set => Set(ref _timePillarProduction, value);
    }

    public double ClosetWorkCost
    {
        get => _closetWorkCost;
        set => Set(ref _closetWorkCost, value);
    }

    public double TimeClosetPrepare
    {
        get => _timeClosetPrepare;
        set => Set(ref _timeClosetPrepare, value);
    }
    
    public async Task LoadFrameDataFromIniAsync()
    {
        var iniData = await CalculationSettingsManager.LoadAsync<FrameSettings, FrameSettingsData>();

        if (iniData == null)
            return;

        iniData.Adapt(this);
    }

    public async Task SaveFrameDataToIniAsync()
    {
        var iniData = this.Adapt<FrameSettingsData>();
        await CalculationSettingsManager.SaveAsync<FrameSettings, FrameSettingsData>(iniData);
    }
}