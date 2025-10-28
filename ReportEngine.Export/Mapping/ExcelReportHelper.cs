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


    public static string EmptyDataString => "N/A";

    public static string CommonErrorString => "Ошибка получения/формирования данных.";


}