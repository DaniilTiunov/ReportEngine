using DocumentFormat.OpenXml;
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


        File.Copy(templatePath, fullSavePath, true);

        //using (var templateDoc = WordprocessingDocument.Open(templatePath, false))
        //{
        //    //создаем новый документ на основе шаблона
        //    using (var newDoc = WordprocessingDocument.Create(fullSavePath, templateDoc.DocumentType))
        //    {
        //        //копируем все части из шаблона в новый документ
        //        foreach (var part in templateDoc.Parts)
        //        {
        //           // newDoc.AddPart(part.OpenXmlPart, part.RelationshipId);
        //        }
        //    }
        //}

        using (var wordDoc = WordprocessingDocument.Open(fullSavePath, true))
        {
            foreach (var stand in project.Stands)
            {
                //заменяем плейсхолдеры в шаблоне
                var replaced = ReplacePlaceholdersText(wordDoc.MainDocumentPart);

                //объединяем в один документ
               // wordDoc.MainDocumentPart.Document.Append(replaced.Document.ChildElements);
            }
        }
    }



    private MainDocumentPart ReplacePlaceholdersText(MainDocumentPart mainPart)
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

            
            //заменяем во всех ключ на значение
            foreach (var descendant in filteredDescendants)
            {
                descendant.Text = descendant.Text.Replace(record.Key, record.Value);
            }

        }

        return mainPart;
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