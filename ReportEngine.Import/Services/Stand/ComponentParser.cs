using ClosedXML.Excel;
using ReportEngine.Import.Configuration;
using ReportEngine.Import.Dto;
using ReportEngine.Import.Services.Excel;

namespace ReportEngine.Import.Services.Stand;

public class ComponentParser
{
    private readonly ComponentsCellConfig _componentsCellConfig = new()
    {
        CategoryName = "ДОПОЛНИТЕЛЬНЫЕ КОМПОНЕНТЫ",
        ColumnPurpose = "B",
        ColumnMaterial = "C",
        ColumnQuantity = "D",
        ColumnCostPerUnit = "E",
        ColumnMeasure = "F",
        ColumnWeight = "G",
        ColumnExportDays = "H"
    };

    private readonly ExcelAddressingService _excelAddressingService;

    public ComponentParser(ExcelAddressingService excelAddressingService)
    {
        _excelAddressingService = excelAddressingService;
    }

    public List<ComponentDto> Parse(IXLWorksheets worksheets)
    {
        var components = new List<ComponentDto>();

        foreach (var worksheet in worksheets.Skip(1))
        {
            var startRow = _excelAddressingService.FindStartRow(
                worksheet,
                _componentsCellConfig.CategoryName) + 2;

            var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 0;

            for (var row = startRow; row <= lastRow; row++)
            {
                var purpose = worksheet
                    .Cell(row, _componentsCellConfig.ColumnPurpose)
                    .GetString();

                if (string.IsNullOrWhiteSpace(purpose))
                    break;

                components.Add(ParseComponent(worksheet, row));
            }
        }

        return components;
    }

    private ComponentDto ParseComponent(IXLWorksheet worksheet, int row)
    {
        return new ComponentDto
        {
            Purpose = worksheet
                .Cell(row, _componentsCellConfig.ColumnPurpose)
                .GetString(),

            Material = worksheet
                .Cell(row, _componentsCellConfig.ColumnMaterial)
                .GetString(),

            Quantity = worksheet
                .Cell(row, _componentsCellConfig.ColumnQuantity)
                .GetString(),

            CostPerUnit = worksheet
                .Cell(row, _componentsCellConfig.ColumnCostPerUnit)
                .GetString(),

            Measure = worksheet
                .Cell(row, _componentsCellConfig.ColumnMeasure)
                .GetString(),

            Weight = worksheet
                .Cell(row, _componentsCellConfig.ColumnWeight)
                .GetString(),

            ExportDays = worksheet
                .Cell(row, _componentsCellConfig.ColumnExportDays)
                .GetString()
        };
    }
}