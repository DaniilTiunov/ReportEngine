namespace ReportEngine.App.Display;

/// <summary>
///     Статический класс, предоставляющий методы для отображения имен свойств оборудования.
/// </summary>
public static class GenericEquipMapper
{
    private static readonly Dictionary<string, string> _equipmentsColumnsName = new()
    {
        ["Name"] = "Название",
        ["Cost"] = "Стоимость",
        ["Measure"] = "Единица измерения",
        ["Height"] = "Высота",
        ["Width"] = "Ширина",
        ["Depth"] = "Глубина",
        ["Weight"] = "Вес",
        ["Type"] = "Тип",
        ["Cabel"] = "Кабель",
        ["ElectricProtection"] = "Электро безопасность",
        ["CabelInput"] = "Кабельный ввод",
        ["ExportDays"] = "Срок поставки",
        ["FormedFrames"] = "Используется в рамах"
    };

    public static string GetColumnName(string propertyName)
    {
        // Проверяем, содержится ли переданное имя свойства в словаре
        if (_equipmentsColumnsName.TryGetValue(propertyName, out var columnName))
            // Если содержится, возвращаем соответствующее отображаемое имя на русском языке
            return columnName;
        // Если соответствие не найдено, возвращаем исходное имя свойства
        return propertyName;
    }
}