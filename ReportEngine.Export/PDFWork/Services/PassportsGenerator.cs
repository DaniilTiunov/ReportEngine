using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Export.ExcelWork.Enums;
using ReportEngine.Export.ExcelWork.Services.Interfaces;
using ReportEngine.Export.Helpers;
using ReportEngine.Shared.Config.Directory;
using ReportEngine.Shared.Config.IniHeleprs;


namespace ReportEngine.Export.PDFWork.Services;

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



        var templatePath = "C:\\Users\\pr-naladka14\\Source\\Repos\\ReportEngine\\ReportEngine.Export\\PDFWork\\Passport_template.docx";

        //открываем шаблон
        using (var templateDoc = WordprocessingDocument.Open(templatePath, false))
        {

            //создаем новый документ
            using (var newDoc = WordprocessingDocument.Create(fullSavePath, templateDoc.DocumentType))
            {


                
                newDoc.AddMainDocumentPart();        
                newDoc.MainDocumentPart.Document = new Document();

               // var templateDocSettings = templateDoc.MainDocumentPart.Document.Body.Descendants<SectionProperties>().First();

                var docSettings = OpenXmlHelper.CreateDefaultPageSettings();
                newDoc.MainDocumentPart.Document.Body = new Body(docSettings);

              

             

                foreach (var stand in project.Stands)
                {
                    //копируем шаблон
                    var standDoc = templateDoc.Clone();

                    //заменяем плейсхолдеры в шаблоне
                    ReplacePlaceholdersText(standDoc.MainDocumentPart);

                    // объединяем в один документ
                    MergeDocuments(newDoc, standDoc);

                }

                newDoc.Save();

            }
        }
    }



    private void ReplacePlaceholdersText(MainDocumentPart mainPart)
    {

        Dictionary<string, string> replacements = new Dictionary<string, string>()
        {
            { "{{stand KKS code}}", "KKS-код стенда" },
            { "{{stand_Name}}", "Наименование стенда" },
            { "{{stand_Manufacturer}}", "Изготовитель стенда" },
            { "{{stand_SerialNumber}}", "Заводской номер стенда" },
            { "{{stand_YearManufacture}}", "Год изготовления стенда" },
            { "{{stand_Description}}", "Описание стенда" }
        };


        foreach (var record in replacements)
        {

            //фильтруем все текстовые элементы, содержащие ключ
            var filteredDescendants = mainPart.Document.Body.Descendants<Text>();
            ;

            //заменяем во всех ключ на значение
            foreach (var descendant in filteredDescendants)
            {
                descendant.Text = descendant.Text.Replace(record.Key, record.Value);
            }

        }

    }


    private void MergeDocuments(WordprocessingDocument mainDoc, WordprocessingDocument docToAdd)
    {
        AlternativeFormatImportPart chunk = mainDoc.MainDocumentPart.AddAlternativeFormatImportPart(
                AlternativeFormatImportPartType.WordprocessingML);


        using (var stream = new MemoryStream())
        {
            docToAdd.Clone(stream);
            //docToAdd.MainDocumentPart.Document.Save(stream);


            stream.Position = 0;

            chunk.FeedData(stream);

            AltChunk altChunk = new AltChunk();


            altChunk.Id = mainDoc.MainDocumentPart.GetIdOfPart(chunk);

            mainDoc.MainDocumentPart.Document.Body.AppendChild(altChunk);
        }

        



    }




    public void CreateStandTitlePage(MainDocumentPart mainDocument, Stand stand)
    {
        var paragraph = new Paragraph();


        // var splitterTable = OpenXmlHelper.AddVerticalLineWithTable();
        //paragraph.AppendChild(splitterTable);


        //выводим лого EAC
        var eacLogoPath = DirectoryHelper.GetImagesRootPath("EAC");

        if (OpenXmlHelper.IsFilePathValid(eacLogoPath))
        {
            var eacImage = OpenXmlHelper.CreateImage(mainDocument, eacLogoPath, 0.5);
            paragraph.AppendChild(eacImage);
        }

        //выводим лого Эталона
        var etalonLogoPath = DirectoryHelper.GetImagesRootPath("Etalon");

        if (OpenXmlHelper.IsFilePathValid(etalonLogoPath))
        {
            var etalonImage = OpenXmlHelper.CreateImage(mainDocument, etalonLogoPath, 0.5);
            paragraph.AppendChild(etalonImage);
        }

        var element = OpenXmlHelper.CreateLineBreak();
        paragraph.AppendChild(element);


        //выводим заголовок паспорта
        element = OpenXmlHelper.CreateTextLine("Стенд датчиков КИПиА");
        paragraph.AppendChild(element);
        element = OpenXmlHelper.CreateLineBreak();
        paragraph.AppendChild(element);

        element = OpenXmlHelper.CreateTextLine($"{stand.KKSCode}");
        paragraph.AppendChild(element);
        element = OpenXmlHelper.CreateLineBreak();
        paragraph.AppendChild(element);

        element = OpenXmlHelper.CreateTextLine("ПАСПОРТ");
        paragraph.AppendChild(element);
        element = OpenXmlHelper.CreateLineBreak();
        paragraph.AppendChild(element);





        //вставляем разрыв страницы
        element = OpenXmlHelper.CreatePageBreak();
        paragraph.AppendChild(element);





        mainDocument.Document.AppendChild(paragraph);
    }

    public int CreateStandPasportBody(Stand stand, int docStartPage)
    {
        var docPageNumber = docStartPage;


        docPageNumber++;
        return docPageNumber;
    }



}