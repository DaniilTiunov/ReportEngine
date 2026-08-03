using ClosedXML.Excel;
using Microsoft.Extensions.DependencyInjection;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Entities.Pipes;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Export.DTO;
using ReportEngine.Shared.Config.IniHeleprs;
using ReportEngine.Shared.Helpers;

namespace ReportEngine.Export.ExcelWork.Services.Generators;

public class FlatSummaryReportGenerator
{
    private readonly IGenericBaseRepository<StainlessPipe, StainlessPipe> _pipesRepository;
    private readonly IProjectInfoRepository _projectInfoRepository;
    private readonly IEnumerable<StainlessPipe> _stainlessPipes;

    public FlatSummaryReportGenerator(
        IProjectInfoRepository projectInfoRepository,
        IServiceProvider serviceProvider)
    {
        _projectInfoRepository = projectInfoRepository;
        _pipesRepository = serviceProvider.GetRequiredService<IGenericBaseRepository<StainlessPipe, StainlessPipe>>();
    }

    public async Task GenerateAsync(int projectId, List<Stand>? selectedStands = null)
    {
        var project = await _projectInfoRepository.GetByIdAsync(projectId);
        var pipes = await _pipesRepository.GetAllAsync();

        using (var wb = new XLWorkbook())
        {
            // Сводная ведомость
            var summarySheet = wb.Worksheets.Add("1C");

            await FillCommonListTable(summarySheet, project, pipes, selectedStands);

            summarySheet.Columns().Style.Alignment.WrapText = false;
            summarySheet.Rows().Style.Alignment.WrapText = false;
            summarySheet.Columns().AdjustToContents();
            summarySheet.Rows().AdjustToContents();

            // Применяем оформление ко всему документу
            foreach (var ws in wb.Worksheets) ws.Cells().Style.Font.FontName = "Times New Roman";

            var savePath = SettingsManager.GetReportDirectory();
            var fileName = ExcelReportHelper.CreateReportName("Сводная ведомость", "xlsx");
            var fullSavePath = Path.Combine(savePath, fileName);

            wb.SaveAs(fullSavePath);
        }
    }

    private async Task FillCommonListTable(
        IXLWorksheet ws,
        ProjectInfo project,
        IEnumerable<StainlessPipe> pipes,
        List<Stand>? selectedStands = null)
    {
        var activeRow = 1;

        var generatedPartsData = selectedStands == null
            ? ExcelReportHelper.GeneratePartsData(project.Stands)
            : ExcelReportHelper.GeneratePartsData(selectedStands);

        // Все комплектующие одним списком
        var allParts = ExcelReportHelper.GenerateAllPartsCollection(generatedPartsData);
        activeRow = FillSubtableData(activeRow, allParts, ws);
    }

    private int FillSubtableData(int startRow, List<EquipmentRecord?> items, IXLWorksheet ws)
    {
        var currentRow = startRow;

        foreach (var item in items)
        {
            if (item == null)
                continue;

            PasteRecord(currentRow, item, ws);

            ws.Cell($"A{currentRow}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            ws.Cell($"B{currentRow}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            ws.Cell($"C{currentRow}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            currentRow++;
        }

        return currentRow;
    }

    private void PasteRecord(int row, EquipmentRecord? record, IXLWorksheet ws)
    {
        ws.Cell($"A{row}").Value = record?.Name.Value;

        ws.Cell($"B{row}").Value = record?.Unit.Value;

        ws.Cell($"C{row}").Value = record?.Quantity.Value?.Round(1).ToString();
    }
}
