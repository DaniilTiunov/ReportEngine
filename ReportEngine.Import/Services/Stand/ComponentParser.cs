using ClosedXML.Excel;
using ReportEngine.Import.Configuration;
using ReportEngine.Import.Dto;
using ReportEngine.Import.Services.Excel;

namespace ReportEngine.Import.Services.Stand;

public class ComponentParser
{
    private readonly ComponentsCellConfig _componentsCellConfig = new()
    {
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

    public List<ComponentDto> Parse(
        IXLWorksheet worksheet,
        string categoryName)
    {
        var components = new List<ComponentDto>();

        var startRow = _excelAddressingService.FindSectionStartRow(
            worksheet, categoryName) + 2;

        var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? 0;

        for (var row = startRow; row <= lastRow; row++)
        {
            var component = ParseComponent(worksheet, row);

            if (component.IsEmpty())
                break;

            components.Add(component);
        }

        return components;
    }

    public ComponentDto ParseComponent(IXLWorksheet worksheet, int row)
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