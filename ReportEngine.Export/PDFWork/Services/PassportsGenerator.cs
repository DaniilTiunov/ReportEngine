using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Export.ExcelWork.Enums;
using ReportEngine.Export.ExcelWork.Services.Interfaces;
using ReportEngine.Shared.Config.Directory;
using ReportEngine.Shared.Config.IniHeleprs;

namespace ReportEngine.Export.PDFWork.Services;

public class PassportsGenerator : IReportGenerator
{
    private readonly IProjectInfoRepository _projectRepository;
    private string savePath = SettingsManager.GetReportDirectory();
    
    private string fileName = "Test.docx";
    public ReportType Type => ReportType.PassportsReport;

    public PassportsGenerator(IProjectInfoRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public Task GenerateAsync(int projectId)
    {
        string filePath = Path.Combine(savePath, fileName);

        using (WordprocessingDocument wordDoc = WordprocessingDocument.Create(
                   filePath, WordprocessingDocumentType.Document))
        {
            // Добавляем основную часть документа
            MainDocumentPart mainPart = wordDoc.AddMainDocumentPart();
            mainPart.Document = new Document();
            Body body = mainPart.Document.AppendChild(new Body());

            // Добавляем абзац с текстом
            Paragraph para = body.AppendChild(new Paragraph());
            Run run = para.AppendChild(new Run());
            run.AppendChild(new Text("Hello, Open XML SDK!"));

            // Сохраняем изменения
            mainPart.Document.Save();
        }
        
        return Task.CompletedTask;
    }
}