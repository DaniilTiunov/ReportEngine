using DocumentFormat.OpenXml.Spreadsheet;
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
            var options = new StringReplaceTextOptions
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
        var opt = new StringReplaceTextOptions
        {
            SearchValue = pictureMarker,
            NewValue = "",
            EscapeRegEx = false
        };

        findedParagraph.ReplaceText(opt);

    }

    private void InsertTablesInTemplate(XceedDocx templateDoc, Stand stand)
    {
        string framesCollectionPrefix = "frames";

        var framesCollectionPostfixs = new List<string>()
        {
            "size",
            "doc_name",
            "quantity"
        };



        //вытаскиваем все записи по рамам
        var framesRecords = stand.StandFrames
            .Select(frame => new Dictionary<string, string>()
            {
                { "size", frame.Frame.Width.ToString() },
                { "doc_name", "N/A"},
                { "quantity", "1"}
            });



        //группируем все записи по ключам в словарях
        var gpoupedFrameRecords = framesRecords
            .SelectMany(frameRecord => frameRecord)
            .GroupBy(kvp => kvp.Key);




        //формируем записи для каждой колонки таблицы
        var columns = framesCollectionPostfixs
            .Select(postfix => new
            {
                //добавляем полную текстовую метку
                postfixMark = postfix,
                fullMark = "{{" + $"{framesCollectionPrefix}.{postfix}" + "}}",
            })
            .Select(markInfo => new
            {
                //ищем место меток в документе
                markInfo.postfixMark,
                markInfo.fullMark,
                placeToInsert = templateDoc.Paragraphs
                    .Where(p => p.Text.Contains(markInfo.fullMark))
                    .First()
            })
            .Select(column => new
            {
                //набиваем колонку данными
                column.postfixMark,
                column.fullMark,
                column.placeToInsert,
                columnData = gpoupedFrameRecords
                        .First(group => group.Key == column.postfixMark)
                        .Select(choicedGroup => choicedGroup.Value)
            })
            .Select(column => new { 

                //склеиваем данные в единую строку для каждого столбца (опционально)
                column.postfixMark,
                column.fullMark,
                column.placeToInsert,
                dataToInsert = string.Join(Environment.NewLine,column.columnData)
            });



        foreach (var column in columns)
        {

            var options = new StringReplaceTextOptions
            {
                SearchValue = column.fullMark ?? string.Empty,
                NewValue = column.dataToInsert ?? string.Empty,
                EscapeRegEx = false
            };


            templateDoc.ReplaceText(options);


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