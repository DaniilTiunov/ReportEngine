using ReportEngine.Shared.Config.IniHeleprs.CalculationSettings.Interfaces;

namespace ReportEngine.Shared.Config.IniHelpers.CalculationSettingsData;

public class StandSettingsData : IIniData
{
    public string? SteelChannel { get; set; } // Швеллер
    public string? SteelChannelEntityName { get; set; } // Швеллер - имя (тип) сущности
    public string? NamePlate { get; set; } // Шильдик
    public string? NamePlateEntityName { get; set; } // Шильдик - имя (тип) сущности
    public string? NameTable { get; set; } // Табличка
    public string? NameTableEntityName { get; set; } // Табличка - имя (тип) сущности
    public string? FrameGalvanizing { get; set; } // Оцинкование рамы
    public string? Bracket { get; set; } // Кронштейн
    public string? BracketEntityName { get; set; } // Кронштейн - имя (тип) сущности
    public string? BracketForDif { get; set; } // Кронштейн для перепадника
    public string? BracketForDifEntityName { get; set; } // Кронштейн для перепадника - имя (тип) сущности
    public string? BracketForAbs { get; set; } // Кронштейн для абсолютника
    public string? BracketForAbsEntityName { get; set; } // Кронштейн для абсолютника - имя (тип) сущности
    public string? BracketUniversal { get; set; } // Кронштейн универсальный
    public string? BracketUniversalEntityName { get; set; } // Кронштейн универсальный - имя (тип) сущности
    public string? CabelSixMm { get; set; } // Кабель 6 мм
    public string? CabelSixMmEntityName { get; set; } // Кабель 6 мм - имя (тип) сущности
    public string? CabelFourMm { get; set; } // Кабель 4 мм
    public string? CabelFourMmEntityName { get; set; } // Кабель 4 мм - имя (тип) сущности
    public string? SignalCable { get; set; } // Кабель сигнальный
    public string? SignalCableEntityName { get; set; } // Кабель сигнальный - имя (тип) сущности
    public string? SeniorEngineer { get; set; } // Ведущий инженер
    public string? ResponsibleForAccept { get; set; } // Отвественный за приёмку
    public string? SecondLevelSpecialist { get; set; } // Специалист 2-го уровня
    public string? OSiL { get; set; } // Представитель ОСиЛ
    public double? SensorCountOnFrame { get; set; } // Кол-во кабеля на 1 раму
    public string? Clamp { get; set; } // Хомут
    public string? ClampEntityName { get; set; } // Хомут - имя (тип) сущности
    public string? Terminal { get; set; } //Клемма
    public string? TerminalEntityName { get; set; } //Клемма - имя (тип) сущности
}
