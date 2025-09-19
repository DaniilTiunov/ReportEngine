using ReportEngine.Shared.Config.IniHeleprs.CalculationSettings.Interfaces;

namespace ReportEngine.Shared.Config.IniHeleprs.CalculationSettingsData;

public class StandSettingsData : IIniData
{
    public string? SteelChannel { get; set; } // Швеллер
    public string? NamePlate { get; set; } // Шильдик
    public string? NameTable { get; set; } // Табличка
    public string? FrameGalvanizing { get; set; } // Оцинкование рамы
    public string? Bracket { get; set; } // Кронштейн
    public string? CabelSixMM { get; set; } // Кабель 6 мм
    public string? CabelFourMM { get; set; } // Кабель 4 мм
    public string? SignalCable { get; set; } // Кабель сигнальный
    public string? SeniorEngineer { get; set; } // Ведущий инженер
    public string? ResponsibleForAccept { get; set; } // Отвественный за приёмку
    public string? SecondLevelSpecialist { get; set; } // Специалист 2-го уровня
    public string? OSiL { get; set; } // Представитель ОСиЛ
    public double? SensorCountOnFrame { get; set; } // Кол-во кабеля на 1 раму
}