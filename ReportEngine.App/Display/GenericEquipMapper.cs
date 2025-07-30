namespace ReportEngine.App.Display
{
    public static class GenericEquipMapper
    {
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
        };
        public static string GetColumnName(string propertyName)
        {
            if (_equipmentsColumnsName.TryGetValue(propertyName, out var columnName))
            {
                return columnName;
            }
            return propertyName;
        }
    }
}
