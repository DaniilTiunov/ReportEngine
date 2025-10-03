namespace ReportEngine.Export.Mapping
{
    public static class TemplateMapper
    {
        public readonly static Dictionary<string, object> PassportMapping = new() {
            { "stand_KKS_code", "KKS-код стенда" },
            { "stand_Name", "Наименование стенда" },
            { "stand_Manufacturer", "Изготовитель стенда" },
            { "stand_SerialNumber", "Заводской номер стенда" },
            { "stand_YearManufacture", "Год изготовления стенда" },
            { "stand_Description", "Описание стенда" }
        };
    };

}
