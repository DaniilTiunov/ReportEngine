using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Export.ExcelWork.Enums;
using ReportEngine.Export.ExcelWork.Services.Interfaces;
using ReportEngine.Export.Mapping;
using ReportEngine.Shared.Config.Directory;
using ReportEngine.Shared.Config.IniHeleprs;
using Xceed.Document.NET;
using XceedDocx = Xceed.Words.NET.DocX;


namespace ReportEngine.Export.PDFWork.Services.Generators;

public class PassportsGenerator : IReportGenerator
{
    private readonly IProjectInfoRepository _projectInfoRepository;

    public PassportsGenerator(IProjectInfoRepository projectRepository)
    {
        _projectInfoRepository = projectRepository;
    }

    public ReportType Type => ReportType.PassportsReport;

    public async Task GenerateAsync(int projectId)
    {
        var project = await _projectInfoRepository.GetByIdAsync(projectId);

        var savePath = SettingsManager.GetReportDirectory();

        var fileName = "Паспорта___" + DateTime.Now.ToString("dd-MM-yy___HH-mm-ss") + ".docx";

        var fullSavePath = Path.Combine(savePath, fileName);

        var templatePath = DirectoryHelper.GetReportsTemplatePath("Passport_template", ".docx");


        using (var myDoc = XceedDocx.Load(templatePath))
        {
            XceedDocx resultDoc = null;  

            foreach (var stand in project.Stands)
            {
                var replacedTemplatedDoc = (XceedDocx)myDoc.Copy();
                ReplaceTextInTemplate(replacedTemplatedDoc, stand);
                resultDoc = resultDoc == null ? replacedTemplatedDoc : MergeDocuments(resultDoc, replacedTemplatedDoc);
            }


            //resultDoc может быть null — в таком случае сохраним пустую копию шаблона
            if (resultDoc == null)
            {
                using var emptyCopy = (XceedDocx)myDoc.Copy();
                emptyCopy.SaveAs(fullSavePath);
            }
            else
            {
                resultDoc.SaveAs(fullSavePath);
            }
        }
    }


    //заменяем метки в документе
    private XceedDocx ReplaceTextInTemplate(XceedDocx templateDoc, Stand stand)
    {
        var replacements = TemplateMapper.GetPassportMapping(stand);

        foreach (var replacement in replacements)
        {
            var options = new StringReplaceTextOptions
            {
                SearchValue = replacement.Key ?? string.Empty,
                NewValue = replacement.Value ?? string.Empty,
                EscapeRegEx = false
            };

            templateDoc.ReplaceText(options);
        }

        return templateDoc;
    }

    //склеиваем два документа в один
    private XceedDocx MergeDocuments(XceedDocx targetDocument, XceedDocx documentToAdd)
    {
        using var ms = new MemoryStream();
        targetDocument.SaveAs(ms); 

        var resultingDoc = XceedDocx.Load(ms);
        resultingDoc.InsertDocument(documentToAdd);

        return resultingDoc;
    }
}