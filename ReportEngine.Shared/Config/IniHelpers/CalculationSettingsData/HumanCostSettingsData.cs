using ReportEngine.Shared.Config.IniHeleprs.CalculationSettings.Interfaces;

namespace ReportEngine.Shared.CalculationSettingsData;

public class HumanCostSettingsData : IIniData
{
    public double ObvzyakaProduction { get; set; } // Изготовление обвязок
    public double CollectorProduction { get; set; } // Изготовление коллектора
    public double Tests { get; set; } // Испытания
    public double CommonCheckStand { get; set; } // Общая проверка стенда
    public double TimeForCheckStand { get; set; } // Время на проверку 1 стенда
    public double TimeForFinalWork { get; set; }  // Время на финальную работу
    public double TimeForOneDrill { get; set; } // Время на сверление 1 отверстия
    public double TimeForCollectorBoil { get; set; } // Время варки коллектора
    public double TimeForAllChecks { get; set; } // Время проведения всех испытаний
    public double TimeForPrepareAllEquipment { get; set; } // Время подготовки всего оборудования
    public double TimeForDrillOneBus { get; set; } // Время на сверления одной шины 
    public double TimeForMontageOneInput { get; set; } // Время монтажа одного ввода
    public double TimeForOthersOperations { get; set; }  // Время на другие операции
}