using ClosedXML.Excel;

namespace ReportEngine.Extensions.Extensions.ClosedXmlExtensions;

public static class CellExtensions
{
    public static string GetCellValue(this IXLWorksheet worksheet, string cellAddress)
    {
        if (string.IsNullOrWhiteSpace(cellAddress))
            return string.Empty;
        
        return worksheet
            .Cell(cellAddress)
            .GetString()
            .Trim();
    }
}