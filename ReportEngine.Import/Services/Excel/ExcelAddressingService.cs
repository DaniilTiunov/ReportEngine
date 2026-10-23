using ClosedXML.Excel;

namespace ReportEngine.Import.Services.Excel;

public class ExcelAddressingService
{
    public int FindSectionLastRow()
    {
        return 1;
    }
    
    public int FindSectionStartRow(
        IXLWorksheet worksheet,
        string categoryName)
    {
        ArgumentNullException.ThrowIfNull(worksheet);

        var cell = worksheet.CellsUsed()
            .FirstOrDefault(cell => string.Equals(
                cell.GetString().Trim(),
                categoryName.Trim(),
                StringComparison.OrdinalIgnoreCase));

        if (cell is null)
        {
            throw new InvalidOperationException(
                $"На листе '{worksheet.Name}' " +
                $"не найдена категория '{categoryName}'.");
        }

        return cell.Address.RowNumber;
    }
}