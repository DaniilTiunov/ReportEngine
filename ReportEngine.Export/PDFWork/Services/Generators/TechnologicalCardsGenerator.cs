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

public class TechnologicalCardsGenerator : IReportGenerator
{
    private static readonly string _savePath = SettingsManager.GetReportDirectory();

    private readonly IProjectInfoRepository _projectInfoRepository;

    public TechnologicalCardsGenerator(IProjectInfoRepository projectInfoRepository)
    {
        _projectInfoRepository = projectInfoRepository;
    }


    public ReportType Type => ReportType.TechnologicalCards;


    public async Task GenerateAsync(int projectId)
    {
        var project = await _projectInfoRepository.GetByIdAsync(projectId);

        var savePath = SettingsManager.GetReportDirectory();

        var fileName = "Технологические карты___" + DateTime.Now.ToString("dd-MM-yy___HH-mm-ss") + ".docx";

        var fullSavePath = Path.Combine(savePath, fileName);

        var templatePath = DirectoryHelper.GetReportsTemplatePath("TechnologicalCards_template", ".docx");


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

        //меняем на картинку
        var pictureMarker = "{{stand_blueprint}}";

        var findedParagraph = templateDoc.Paragraphs
            .Where(p => p.Text.Contains(pictureMarker))
            .First();


        //подгружаем картинку
        using (var imageMemoryStream = new MemoryStream(stand.ImageData))
        {
            var img = templateDoc.AddImage(imageMemoryStream);
            var picture = img.CreatePicture();


            findedParagraph.InsertPicture(picture);
        }

        //убираем текстовый маркер 
        var opt = new StringReplaceTextOptions
        {
            SearchValue = pictureMarker,
            NewValue = "",
            EscapeRegEx = false
        };

        findedParagraph.ReplaceText(opt);


        return templateDoc;
    }


    private XceedDocx MergeDocuments(XceedDocx targetDocument, XceedDocx documentToAdd)
    {
        using var ms = new MemoryStream();
        targetDocument.SaveAs(ms); // сериализуем targetDocument в поток

        var resultingDoc = XceedDocx.Load(ms); // загружаем независимый экземпляр
        resultingDoc.InsertDocument(documentToAdd);
        return resultingDoc;
    }
}