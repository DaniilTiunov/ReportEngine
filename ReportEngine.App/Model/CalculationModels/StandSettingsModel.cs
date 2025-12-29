using Mapster;
using ReportEngine.App.ViewModels;
using ReportEngine.App.ViewModels.DTO;
using ReportEngine.Shared.Config.IniHelpers;
using ReportEngine.Shared.Config.IniHelpers.CalculationSettings;
using ReportEngine.Shared.Config.IniHelpers.CalculationSettingsData;

namespace ReportEngine.App.Model.CalculationModels;

public class StandSettingsModel : BaseViewModel
{
    private string? _bracket;
    private string? _bracketMeasure;

    private string? _bracketForDif;
    private string? _bracketForDifMeasure;

    private string? _bracketForAbs;
    private string? _bracketForAbsMeasure;

    private string? _bracketUniversal;
    private string? _bracketUniversalMeasure;

    private string? _cabelFourMm;
    private string? _cabelFourMmMeasure;

    private string? _cabelSixMm;
    private string? _cabelSixMmMeasure;

    private double? _sensorCountOnFrame;

    private string? _frameGalvanizing;

    private string? _namePlate;
    private string? _namePlateMeasure;

    private string? _nameTable;
    private string? _nameTableMeasure;

    private string? _oSiL;
    private string? _responsibleForAccept;
    private string? _secondLevelSpecialist;
    private string? _seniorEngineer;
    

    private string? _signalCable;
    private string? _signalCableMeasure;

    private string? _steelChannel;
    private string? _steelChannelMeasure;

    private string? _clamp;
    private string? _clampMeasure;

    private string? _terminal;
    private string? _terminalMeasure;

    public string? SteelChannel
    {
        get => _steelChannel;
        set => Set(ref _steelChannel, value);
    } // Швеллер

    public string? SteelChannelMeasure
    {
        get => _steelChannelMeasure;
        set => Set(ref _steelChannelMeasure, value);
    } // Швеллер - ед. изм.

    public string? NamePlate
    {
        get => _namePlate;
        set => Set(ref _namePlate, value);
    } // Шильдик

    public string? NamePlateMeasure
    {
        get => _namePlateMeasure;
        set => Set(ref _namePlateMeasure, value);
    } // Шильдик - ед.изм

    public string? NameTable
    {
        get => _nameTable;
        set => Set(ref _nameTable, value);
    } // Табличка

    public string? NameTableMeasure
    {
        get => _nameTableMeasure;
        set => Set(ref _nameTableMeasure, value);
    } // Табличка - ед.изм

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

    public string? BracketMeasure
    {
        get => _bracketMeasure;
        set => Set(ref _bracketMeasure, value);
    } // Кронштейн - ед.изм

    public string? BracketForDif
    {
        get => _bracketForDif;
        set => Set(ref _bracketForDif, value);
    } // Кронштейн для перепадника

    public string? BracketForDifMeasure
    {
        get => _bracketForDifMeasure;
        set => Set(ref _bracketForDifMeasure, value);
    } // Кронштейн для перепадника - ед.изм

    public string? BracketForAbs
    {
        get => _bracketForAbs;
        set => Set(ref _bracketForAbs, value);
    } // Кронштейн для абсолютника

    public string? BracketForAbsMeasure
    {
        get => _bracketForAbsMeasure;
        set => Set(ref _bracketForAbsMeasure, value);
    } // Кронштейн для абсолютника - ед. изм

    public string? BracketUniversal
    {
        get => _bracketUniversal;
        set => Set(ref _bracketUniversal, value);
    } // Кронштейн универсальный

    public string? BracketUniversalMeasure
    {
        get => _bracketUniversalMeasure;
        set => Set(ref _bracketUniversalMeasure, value);
    } // Кронштейн универсальный - ед. изм

    public string? CabelSixMm
    {
        get => _cabelSixMm;
        set => Set(ref _cabelSixMm, value);
    } // Кабель 6 мм

    public string? CabelSixMmMeasure
    {
        get => _cabelSixMmMeasure;
        set => Set(ref _cabelSixMmMeasure, value);
    } // Кабель 6 мм - ед. изм

    public string? CabelFourMm
    {
        get => _cabelFourMm;
        set => Set(ref _cabelFourMm, value);
    } // Кабель 4 мм

    public string? CabelFourMmMeasure
    {
        get => _cabelFourMmMeasure;
        set => Set(ref _cabelFourMmMeasure, value);
    } // Кабель 4 мм - ед. изм

    public string? SignalCable
    {
        get => _signalCable;
        set => Set(ref _signalCable, value);
    } // Кабель сигнальный

    public string? SignalCableMeasure
    {
        get => _signalCableMeasure;
        set => Set(ref _signalCableMeasure, value);
    } // Кабель сигнальный - ед.изм


    public string? SeniorEngineer
    {
        get => _seniorEngineer;
        set => Set(ref _seniorEngineer, value);
    } // Ведущий инженер

    public string? ResponsibleForAccept
    {
        get => _responsibleForAccept;
        set => Set(ref _responsibleForAccept, value);
    } // Ответственный за приёмку

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


    public string? Clamp
    {
        get => _clamp;
        set => Set(ref _clamp, value);
    } // Хомут

    public string? ClampMeasure
    {
        get => _clampMeasure;
        set => Set(ref _clampMeasure, value);
    } // Хомут - ед.изм

    public string? Terminal
    {
        get => _terminal;
        set => Set(ref _terminal, value);
    } // Клемма

    public string? TerminalMeasure
    {
        get => _terminalMeasure;
        set => Set(ref _terminalMeasure, value);
    } // Клемма - ед.изм



    public async Task LoadStandsSettingsDataAsync()
    {
        var iniData = CalculationSettingsManager.Load<StandSettings, StandSettingsData>();
        
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
