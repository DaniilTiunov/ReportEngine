using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Export.ExcelWork.Enums;
using ReportEngine.Export.ExcelWork.Services.Interfaces;
using ReportEngine.Export.Mapping;
using ReportEngine.Export.Mapping.JsonObjects;
using ReportEngine.Shared.Config.Directory;
using ReportEngine.Shared.Config.IniHeleprs;
using System.Text.Json;
using XceedDocx = Xceed.Words.NET.DocX;
using XceedNet = Xceed.Document.NET;

namespace ReportEngine.Export.PDFWork.Services.Generators;

public class TechnologicalCardsGenerator : IReportGenerator
{


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

        var fileName = ExcelReportHelper.CreateReportName("Технологические карты", ".docx");
     
        var fullSavePath = Path.Combine(savePath, fileName);

        var templatePath = DirectoryHelper.GetReportsTemplatePath("TechnologicalCards_template", ".docx");


        var dataObject = ExcelReportHelper.CreateProjectJson(project);
        var jsonObject = JsonSerializer.Serialize(dataObject);
        

        using (var myDoc = XceedDocx.Load(templatePath))
        {
            XceedDocx resultDoc = null;

            foreach (var stand in project.Stands)
            {
                var replacedTemplatedDoc = (XceedDocx)myDoc.Copy();
                ReplaceTextInTemplate(replacedTemplatedDoc, stand);
                InsertBlueprintInTemplate(replacedTemplatedDoc, stand);
                InsertTablesInTemplate(replacedTemplatedDoc, stand);
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


    private void ReplaceTextInTemplate(XceedDocx templateDoc, Stand stand)
    {
        var replacements = TemplateMapper.GetPassportMapping(stand);

        foreach (var replacement in replacements)
        {
            var options = new XceedNet.StringReplaceTextOptions
            {
                SearchValue = replacement.Key ?? string.Empty,
                NewValue = replacement.Value ?? string.Empty,
                EscapeRegEx = false
            };

            templateDoc.ReplaceText(options);
        }
    }


    private void InsertBlueprintInTemplate(XceedDocx templateDoc, Stand stand)
    {
        //ищем параграф с маркером
        var pictureMarker = "{{stand_blueprint}}";

        var findedParagraph = templateDoc.Paragraphs
            .Where(p => p.Text.Contains(pictureMarker))
            .First();

        //подгружаем картинку
        using (var imageMemoryStream = new MemoryStream(stand.ImageData))
        {
            var img = templateDoc.AddImage(imageMemoryStream);
            var picture = img.CreatePicture();
            picture.Height = 400;
            picture.Width = 230;


            findedParagraph.InsertPicture(picture);
        }

        //убираем текстовый маркер 
        var options = new XceedNet.StringReplaceTextOptions
        {
            SearchValue = pictureMarker,
            NewValue = "",
            EscapeRegEx = false
        };

        findedParagraph.ReplaceText(options);
    }


    private IEnumerable<XceedNet.Table> GetTablesByPrefix(XceedDocx templateDoc, string prefix)
    {
        return templateDoc.Tables
            .Where(table => table.Paragraphs
                .Any(par => par.Text.Contains(prefix)));
    }


    private void InsertTablesInTemplate(XceedDocx templateDoc, Stand stand)
    {
        var framesCollectionPrefix = "frames";

        var framesCollectionPostfixs = new List<string>
        {
            "size",
            "doc_name",
            "quantity"
        };


        //формируем все записи по рамам
        var framesTableRecords = stand.StandFrames
            .Select(frame => new Dictionary<string, string>
            {
                { "size", frame.Frame.Width.ToString() },
                { "doc_name", "N/A" },
                { "quantity", "1" }
            });

        var frameTable = GetTablesByPrefix(templateDoc, framesCollectionPrefix).First();


        //разворачиваем в колонки
        var columns = new Dictionary<string, IEnumerable<string>>
        {
            { "size", framesTableRecords.Select(dict => dict["size"]) },
            { "doc_name", framesTableRecords.Select(dict => dict["doc_name"]) },
            { "quantity", framesTableRecords.Select(dict => dict["quantity"]) }
        };


        var marksInfo = framesCollectionPostfixs
            .Select(postfix => new
            {
                //добавляем полную текстовую метку
                postfixMark = postfix,
                fullMark = "{{" + $"{framesCollectionPrefix}.{postfix}" + "}}"
            })
            .Select(mark => new
            {
                //ищем место меток в документе
                mark.postfixMark,
                mark.fullMark,
                placesToInsert = templateDoc.Paragraphs
                    .Where(p => p.Text.Contains(mark.fullMark))
            })
            .Select(mark => new
            {
                mark.postfixMark,
                mark.fullMark,
                mark.placesToInsert,
                data = columns
                    .Where(dict => dict.Key == mark.postfixMark)
                    .SelectMany(dict => dict.Value)
            });


        foreach (var mark in marksInfo)
            foreach (var dataValue in mark.data)
            {
                //заменяем текстовый маркер
                var options = new XceedNet.StringReplaceTextOptions
                {
                    SearchValue = mark.fullMark,
                    NewValue = dataValue,
                    EscapeRegEx = false
                };


                //в каждом найденном месте меняем
                foreach (var place in mark.placesToInsert) place.ReplaceText(options);
            }
    }


    private XceedDocx MergeDocuments(XceedDocx targetDocument, XceedDocx documentToAdd)
    {
        // сериализуем targetDocument в поток
        using var ms = new MemoryStream();
        targetDocument.SaveAs(ms);

        // загружаем независимый экземпляр
        var resultingDoc = XceedDocx.Load(ms);
        resultingDoc.InsertDocument(documentToAdd);

        return resultingDoc;
    }
}