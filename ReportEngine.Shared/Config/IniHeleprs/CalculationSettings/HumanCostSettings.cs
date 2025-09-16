namespace ReportEngine.Shared.CalculationSettings;

public class HumanCostSettings
{
    public float ObvzyakaProduction { get; set; } // Изготовление обвязок
    public float CollectorProduction { get; set; } // Изготовление коллектора
    public float Tests { get; set; } // Испытания
    public float CommonCheckStand { get; set; } // Общая проверка стенда
    public float TimeForCheckStand { get; set; } // Время на проверку 1 стенда
    public float TimeForFinalWork { get; set; }  // Время на финальную работу
    public float TimeForOneDrill { get; set; } // Время на сверление 1 отверстия
    public float TimeForCollectorBoil { get; set; } // Время варки коллектора
    public float TimeForAllChecks { get; set; } // Время проведения всех испытаний
    public float TimeForPrepareAllEquipment { get; set; } // Время подготовки всего оборудования
    public float TimeForDrillOneBus { get; set; } // Время на сверления одной шины 
    public float TimeForMontageOneInput { get; set; } // Время монтажа одного ввода
    public float TimeForOthersOperations { get; set; }  // Время на другие операции
}