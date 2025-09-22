using Mapster;
using ReportEngine.App.ViewModels;
using ReportEngine.Shared.Config.IniHelpers;
using ReportEngine.Shared.Config.IniHelpers.CalculationSettings;
using ReportEngine.Shared.Config.IniHelpers.CalculationSettingsData;

namespace ReportEngine.App.Model.CalculationModels;

public class StandSettingsModel : BaseViewModel
{
    private string? _bracket;
    private string? _cabelFourMM;
    private string? _cabelSixMM;
    private string? _frameGalvanizing;
    private string? _namePlate;
    private string? _nameTable;
    private string? _oSiL;
    private string? _responsibleForAccept;
    private string? _secondLevelSpecialist;
    private string? _seniorEngineer;
    private double? _sensorCountOnFrame;
    private string? _signalCable;
    private string? _steelChannel;

    public string? SteelChannel
    {
        get => _steelChannel;
        set => Set(ref _steelChannel, value);
    } // Швеллер

    public string? NamePlate
    {
        get => _namePlate;
        set => Set(ref _namePlate, value);
    } // Шильдик

    public string? NameTable
    {
        get => _nameTable;
        set => Set(ref _nameTable, value);
    } // Табличка

    public string? FrameGalvanizing
    {
        get => _frameGalvanizing;
        set => Set(ref _frameGalvanizing, value);
    } // Оцинкование рамы

    public string? Bracket
    {
        get => _bracket;
        set => Set(ref _bracket, value);
    } // Кронштейн

    public string? CabelSixMM
    {
        get => _cabelSixMM;
        set => Set(ref _cabelSixMM, value);
    } // Кабель 6 мм

    public string? CabelFourMM
    {
        get => _cabelFourMM;
        set => Set(ref _cabelFourMM, value);
    } // Кабель 4 мм

    public string? SignalCable
    {
        get => _signalCable;
        set => Set(ref _signalCable, value);
    } // Кабель сигнальный

    public string? SeniorEngineer
    {
        get => _seniorEngineer;
        set => Set(ref _seniorEngineer, value);
    } // Ведущий инженер

    public string? ResponsibleForAccept
    {
        get => _responsibleForAccept;
        set => Set(ref _responsibleForAccept, value);
    } // Отвественный за приёмку

    public string? SecondLevelSpecialist
    {
        get => _secondLevelSpecialist;
        set => Set(ref _secondLevelSpecialist, value);
    } // Специалист 2-го уровня

    public string? OSiL
    {
        get => _oSiL;
        set => Set(ref _oSiL, value);
    } // Представитель ОСиЛ

    public double? SensorCountOnFrame
    {
        get => _sensorCountOnFrame;
        set => Set(ref _sensorCountOnFrame, value);
    } // Кол-во кабеля на 1 раму

    public async Task LoadStandsSettingsDataAsync()
    {
        var iniData = await CalculationSettingsManager.LoadAsync<StandSettings, StandSettingsData>();

        if (iniData == null)
            return;

        iniData.Adapt(this);
    }

    public async Task SaveDataToIniAsync()
    {
        var iniData = this.Adapt<StandSettingsData>();
        await CalculationSettingsManager.SaveAsync<StandSettings, StandSettingsData>(iniData);
    }
}