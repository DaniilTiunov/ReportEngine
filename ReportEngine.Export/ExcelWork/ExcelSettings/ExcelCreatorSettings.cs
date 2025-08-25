using ReportEngine.Shared.Config.Directory;

namespace ReportEngine.Export.ExcelWork.ExcelSettings;

public class ExcelCreatorSettings
{
    public string WorksheetName { get; set; } = "Ведомость копмлектующих";
    public string FileName { get; set; } = "Ведомость копмлектующих.xlsx";
    public string SaveDirectory { get; set; } = DirectoryHelper.GetDirectory();
    public Dictionary<int, double> ColumnWidths { get; set; } = new()
    {
        { 1, 3 },
        { 2, 40 },
        { 3, 12 },
        { 4, 8 },
        { 5, 3 }
    };
    public bool OpenAfterSave { get; set; } = true;
}