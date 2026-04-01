namespace ReportEngine.Domain.Entities.CalculationParameters.Enums;

public enum EquipReferenceType
{
    CarbonArmature, // Таблица углеродистые трубы
    HeaterArmature, // Таблица жаропрочные трубы
    StainlessArmature, // Таблица нержавеющие трубы
    BoxesBrace, // Таблицп крепление клеммных коробок
    DrainageBrace, // Таблица Крепление дренажа
    SensorBrace, // Таблица Крепление датчиков
    Drainage, // Таблица дренаж
    CabelBoxe, // Таблица клемные коробки
    CabelInput, // Таблица клемные вводы
    CabelProduction, // Таблица кабельная продукция
    CabelProtection, // Таблица средства прокладки и защиты кабеля
    Heater, // Таблица обогрев
    CarbonSocket, // Таблица тройники и КМЧ углеродистые
    HeaterSocket, // Таблица тройники и КМЧ жаропрочные
    StainlessSocket, // Таблица тройники и КМЧ нержавеющие
    Container, // Таблица упаковок
    Other, // Таблица прочее
    CarbonPipe, // Таблица трубы углеродистые
    HeaterPipe, // Таблица трубы жаропрочные
    StainlessPipe // Таблица трубы нержавеющие
}
