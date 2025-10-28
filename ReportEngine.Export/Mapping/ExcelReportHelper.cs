namespace ReportEngine.Export.Mapping;

public static class ExcelReportHelper
{
    public static string CreateReportName(string prefix, string fileExtension)
    {
        return prefix + "___" + DateTime.Now.ToString("dd-MM-yy___HH-mm-ss") + "." + fileExtension;
    }

    public static float? TryToParseFloat(string str)
    {
        return float.TryParse(str, out float parseResult) ? parseResult : null;
    }


    public static string DbErrorString => "В БД отсутствуют необходимые значения.";

    public static string SettingsErrorString => "Ошибка получения необходимых настроек расчета.";

    public static string UnreliableDataString => "Рассчитанные данные могут быть недостоверны.";

    public static string CommonErrorString => "Ошибка получения/формирования данных.";


}