namespace ReportEngine.App.Display
{
    /// <summary>
    /// Статический класс, предоставляющий методы для отображения имен свойств оборудования.
    /// </summary>
    public static class GenericEquipMapper
    {
        /// <summary>
        /// Словарь, содержащий соответствие между именами свойств на английском языке и их отображаемыми именами на русском языке.
        /// </summary>
        private static readonly Dictionary<string, string> _equipmentsColumnsName = new Dictionary<string, string>
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

        /// <summary>
        /// Возвращает отображаемое имя столбца на русском языке по имени свойства на английском языке.
        /// </summary>
        /// <param name="propertyName">Имя свойства на английском языке.</param>
        /// <returns>Отображаемое имя столбца на русском языке. Если соответствие не найдено, возвращает исходное имя свойства.</returns>
        public static string GetColumnName(string propertyName)
        {
            // Проверяем, содержится ли переданное имя свойства в словаре
            if (_equipmentsColumnsName.TryGetValue(propertyName, out var columnName))
            {
                // Если содержится, возвращаем соответствующее отображаемое имя на русском языке
                return columnName;
            }
            // Если соответствие не найдено, возвращаем исходное имя свойства
            return propertyName;
        }
    }
}
