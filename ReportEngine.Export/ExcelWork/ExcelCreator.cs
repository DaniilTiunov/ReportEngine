using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;
using ClosedXML.Excel;
using ReportEngine.Export.ExcelWork.ExcelSettings;
using ReportEngine.Shared.Config.Directory;

namespace ReportEngine.Export.ExcelWork;

public class ExcelCreator
{
    private readonly IProjectInfoRepository _repository;
    private readonly ExcelCreatorSettings _excelSettings;

    public ExcelCreator(IProjectInfoRepository Repository, ExcelCreatorSettings excelSettings)
    {
        _repository = Repository;
        _excelSettings = excelSettings;
    }

    public async void CreateListOfComponents(int projectId)
    {
        var project = await _repository.GetByIdAsync(projectId);
        string title = project.Company;
        string subTitle = "Сводная ведомость комплектующих";

        using (var workBook = new XLWorkbook())
        {
            var worksheet = workBook.AddWorksheet(_excelSettings.WorksheetName);
            
            worksheet.Column(1).Width = 3;   // небольшой левый отступ
            worksheet.Column(2).Width = 40;  // "Наименование" и основной блок
            worksheet.Column(3).Width = 12;  // "Ед. изм."
            worksheet.Column(4).Width = 8;   // "Кол."
            worksheet.Column(5).Width = 3;   // правый отступ
            
            var wsTitle = worksheet.Range(1, 2, 1, 4).Merge();
            wsTitle.Value = title;
            wsTitle.Style.Font.Bold = true;
            
            var fileName = $"{_excelSettings.WorksheetName}.xlsx + {DateTime.Now:yyyy-MM-dd HH-mm} + {project.Company}";
            var filePath = Path.Combine(DirectoryHelper.GetReportsPath(), fileName);
            workBook.SaveAs(filePath);
        }
    }
}