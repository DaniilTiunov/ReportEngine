using ClosedXML.Excel;

namespace ReportEngine.Import.Services.Excel;

public class ExcelAddressingService
{
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


        if (cell is not null) return cell.Address.RowNumber;

        throw new InvalidOperationException(
            $"На листе '{worksheet.Name}' " +
            $"не найдена категория '{categoryName}'.");
    }


    public (int startRow, int lastRow) FindSectionRows(IXLWorksheet worksheet, string categoryName,
        int? startSearсhRow = 1)
    {
        ArgumentNullException.ThrowIfNull(worksheet);

        var firstSearchRow = startSearсhRow ?? 1; // если начальная строка не передана - ищем с начала листа

        var cell = worksheet.CellsUsed()
            .Where(c => c.Address.RowNumber >= firstSearchRow) // ← фильтр по строке
            .FirstOrDefault(c => string.Equals(
                c.GetString().Trim(),
                categoryName.Trim(),
                StringComparison.OrdinalIgnoreCase));


        var startRow = cell is not null
            ? cell.Address.RowNumber
            : throw new InvalidOperationException(
                $"На листе '{worksheet.Name}' " +
                $"не найдена категория '{categoryName}'.");


        var lastUsedRow = worksheet.LastRowUsed()?.RowNumber() ?? startRow;


        // Ищем первую пустую строку начиная со следующей после заголовка
        var endRow = startRow;
        for (var r = startRow + 1; r <= lastUsedRow; r++)
        {
            var row = worksheet.Row(r);

            if (row.IsEmpty())
                break;

            endRow = r;
        }

        return (startRow, endRow);
    }
}