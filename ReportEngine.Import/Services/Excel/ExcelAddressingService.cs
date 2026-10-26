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


        if (cell is not null)
        {
            return cell.Address.RowNumber;
        }

        throw new InvalidOperationException(
                $"На листе '{worksheet.Name}' " +
                $"не найдена категория '{categoryName}'.");
    }




    public (int startRow,int lastRow) FindSectionRows(IXLWorksheet worksheet, string categoryName)
    {
        ArgumentNullException.ThrowIfNull(worksheet);

        var cell = worksheet.CellsUsed()
            .FirstOrDefault(cell => string.Equals(
                cell.GetString().Trim(),
                categoryName.Trim(),
                StringComparison.OrdinalIgnoreCase));


        var startRow = cell is not null ? cell.Address.RowNumber : throw new InvalidOperationException(
                $"На листе '{worksheet.Name}' " +
                $"не найдена категория '{categoryName}'.");


        var lastUsedRow = worksheet.LastRowUsed()?.RowNumber() ?? startRow;


        // Ищем первую пустую строку начиная со следующей после заголовка
        int endRow = startRow;
        for (int r = startRow + 1; r <= lastUsedRow; r++)
        {
            var row = worksheet.Row(r);

            if (row.IsEmpty())
                break;

            endRow = r;
        }

        return (startRow, endRow);
    }
}
