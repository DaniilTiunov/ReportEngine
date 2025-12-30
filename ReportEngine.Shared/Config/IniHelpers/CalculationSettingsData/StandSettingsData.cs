using ReportEngine.Shared.Config.IniHeleprs.CalculationSettings.Interfaces;

namespace ReportEngine.Shared.Config.IniHelpers.CalculationSettingsData;

public class StandSettingsData : IIniData
{
    public string? SteelChannel { get; set; } // Швеллер
    public string? SteelChannelMeasure { get; set; } // Швеллер - ед.изм
    public string? NamePlate { get; set; } // Шильдик
    public string? NamePlateMeasure { get; set; } // Шильдик - ед. изм
    public string? NameTable { get; set; } // Табличка
    public string? NameTableMeasure { get; set; } // Табличка - ед. изм
    public string? FrameGalvanizing { get; set; } // Оцинкование рамы
    public string? Bracket { get; set; } // Кронштейн
    public string? BracketMeasure { get; set; } // Кронштейн - ед. изм
    public string? BracketForDif { get; set; } // Кронштейн для перепадника
    public string? BracketForDifMeasure { get; set; } // Кронштейн для перепадника - ед .изм
    public string? BracketForAbs { get; set; } // Кронштейн для абсолютника
    public string? BracketForAbsMeasure { get; set; } // Кронштейн для абсолютника - ед .изм
    public string? BracketUniversal { get; set; } // Кронштейн универсальный
    public string? BracketUniversalMeasure { get; set; } // Кронштейн универсальный - ед .изм
    public string? CabelSixMm { get; set; } // Кабель 6 мм
    public string? CabelSixMmMeasure { get; set; } // Кабель 6 мм - ед. изм
    public string? CabelFourMm { get; set; } // Кабель 4 мм
    public string? CabelFourMmMeasure { get; set; } // Кабель 4 мм - ед.изм
    public string? SignalCable { get; set; } // Кабель сигнальный
    public string? SignalCableMeasure { get; set; } // Кабель сигнальный - ед.изм
    public string? SeniorEngineer { get; set; } // Ведущий инженер
    public string? ResponsibleForAccept { get; set; } // Отвественный за приёмку
    public string? SecondLevelSpecialist { get; set; } // Специалист 2-го уровня
    public string? OSiL { get; set; } // Представитель ОСиЛ
    public double? SensorCountOnFrame { get; set; } // Кол-во кабеля на 1 раму
    public string? Clamp {  get; set; } // Хомут
    public string? ClampMeasure { get; set; } // Хомут - ед.изм
    public string? Terminal { get; set; } //Клемма
    public string? TerminalMeasure { get; set; } //Клемма - ед.изм
}