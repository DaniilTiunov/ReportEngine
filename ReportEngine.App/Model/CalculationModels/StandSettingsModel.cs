using Mapster;
using ReportEngine.App.ViewModels;
using ReportEngine.Shared.Config.IniHelpers;
using ReportEngine.Shared.Config.IniHelpers.CalculationSettings;
using ReportEngine.Shared.Config.IniHelpers.CalculationSettingsData;

namespace ReportEngine.App.Model.CalculationModels;

public class StandSettingsModel : BaseViewModel
{
    private string? _bracket;
    private string? _bracketEntityName;

    private string? _bracketForDif;
    private string? _bracketForDifEntityName;

    private string? _bracketForAbs;
    private string? _bracketForAbsEntityName;

    private string? _bracketUniversal;
    private string? _bracketUniversalEntityName;

    private string? _cabelFourMm;
    private string? _cabelFourMmEntityName;

    private string? _cabelSixMm;
    private string? _cabelSixMmEntityName;

    private double? _sensorCountOnFrame;

    private string? _frameGalvanizing;

    private string? _namePlate;
    private string? _namePlateEntityName;

    private string? _nameTable;
    private string? _nameTableEntityName;

    private string? _oSiL;
    private string? _responsibleForAccept;
    private string? _secondLevelSpecialist;
    private string? _seniorEngineer;

    private string? _signalCable;
    private string? _signalCableEntityName;

    private string? _steelChannel;
    private string? _steelChannelEntityName;

    private string? _clamp;
    private string? _clampEntityName;

    private string? _terminal;
    private string? _terminalEntityName;

    public string? SteelChannel
    {
        get => _steelChannel;
        set => Set(ref _steelChannel, value);
    } // Швеллер

    public string? SteelChannelEntityName
    {
        get => _steelChannelEntityName;
        set => Set(ref _steelChannelEntityName, value);
    } // Швеллер - ед. изм.

    public string? NamePlate
    {
        get => _namePlate;
        set => Set(ref _namePlate, value);
    } // Шильдик

    public string? NamePlateEntityName
    {
        get => _namePlateEntityName;
        set => Set(ref _namePlateEntityName, value);
    } // Шильдик - ед.изм

    public string? NameTable
    {
        get => _nameTable;
        set => Set(ref _nameTable, value);
    } // Табличка

    public string? NameTableEntityName
    {
        get => _nameTableEntityName;
        set => Set(ref _nameTableEntityName, value);
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

    public string? BracketEntityName
    {
        get => _bracketEntityName;
        set => Set(ref _bracketEntityName, value);
    } // Кронштейн - ед.изм

    public string? BracketForDif
    {
        get => _bracketForDif;
        set => Set(ref _bracketForDif, value);
    } // Кронштейн для перепадника

    public string? BracketForDifEntityName
    {
        get => _bracketForDifEntityName;
        set => Set(ref _bracketForDifEntityName, value);
    } // Кронштейн для перепадника - ед.изм

    public string? BracketForAbs
    {
        get => _bracketForAbs;
        set => Set(ref _bracketForAbs, value);
    } // Кронштейн для абсолютника

    public string? BracketForAbsEntityName
    {
        get => _bracketForAbsEntityName;
        set => Set(ref _bracketForAbsEntityName, value);
    } // Кронштейн для абсолютника - ед. изм

    public string? BracketUniversal
    {
        get => _bracketUniversal;
        set => Set(ref _bracketUniversal, value);
    } // Кронштейн универсальный

    public string? BracketUniversalEntityName
    {
        get => _bracketUniversalEntityName;
        set => Set(ref _bracketUniversalEntityName, value);
    } // Кронштейн универсальный - ед. изм

    public string? CabelSixMm
    {
        get => _cabelSixMm;
        set => Set(ref _cabelSixMm, value);
    } // Кабель 6 мм

    public string? CabelSixMmEntityName
    {
        get => _cabelSixMmEntityName;
        set => Set(ref _cabelSixMmEntityName, value);
    } // Кабель 6 мм - ед. изм

    public string? CabelFourMm
    {
        get => _cabelFourMm;
        set => Set(ref _cabelFourMm, value);
    } // Кабель 4 мм

    public string? CabelFourMmEntityName
    {
        get => _cabelFourMmEntityName;
        set => Set(ref _cabelFourMmEntityName, value);
    } // Кабель 4 мм - ед. изм

    public string? SignalCable
    {
        get => _signalCable;
        set => Set(ref _signalCable, value);
    } // Кабель сигнальный

    public string? SignalCableEntityName
    {
        get => _signalCableEntityName;
        set => Set(ref _signalCableEntityName, value);
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

    public string? ClampEntityName
    {
        get => _clampEntityName;
        set => Set(ref _clampEntityName, value);
    } // Хомут - ед.изм

    public string? Terminal
    {
        get => _terminal;
        set => Set(ref _terminal, value);
    } // Клемма

    public string? TerminalEntityName
    {
        get => _terminalEntityName;
        set => Set(ref _terminalEntityName, value);
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
