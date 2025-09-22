
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Export.ExcelWork.Enums;
using ReportEngine.Export.ExcelWork.Services.Interfaces;
using ReportEngine.Shared.Config.Directory;
using ReportEngine.Shared.Config.IniHeleprs;
using A = DocumentFormat.OpenXml.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;

namespace ReportEngine.Export.PDFWork.Services;

public class PassportsGenerator : IReportGenerator
{
    private readonly IProjectInfoRepository _projectInfoRepository;

    public ReportType Type => ReportType.PassportsReport;



    public PassportsGenerator(IProjectInfoRepository projectRepository)
    {
        _projectInfoRepository = projectRepository;
    }



    public async Task GenerateAsync(int projectId)
    {
        var project = await _projectInfoRepository.GetByIdAsync(projectId);

        string savePath = SettingsManager.GetReportDirectory();

        string fileName = "Паспорта___" + DateTime.Now.ToString("dd-MM-yy___HH-mm-ss") + ".docx";

        string filePath = Path.Combine(savePath, fileName);

        using (WordprocessingDocument wordDoc = WordprocessingDocument.Create(
                   filePath, WordprocessingDocumentType.Document))
        {


            // Добавляем основную часть документа
            MainDocumentPart mainPart = wordDoc.AddMainDocumentPart();
            mainPart.Document = new Document();
            Body body = mainPart.Document.AppendChild(new Body());

            SetLandscapeOrientation(mainPart);



            foreach (var stand in project.Stands)
            {
                CreateStandTitlePage(mainPart, stand);



            }



            // Сохраняем изменения
            mainPart.Document.Save();

        }


    }






    public void CreateStandTitlePage(MainDocumentPart mainDocument, Stand stand)
    {

        Paragraph paragraph = new Paragraph();



        //выводим лого EAC
        var eacLogoPath = DirectoryHelper.GetImagesRootPath("EAC");
        var eacImage = CreateImage(mainDocument, eacLogoPath);


        paragraph.AppendChild(eacImage);

        var element = CreateLineBreak();
        paragraph.AppendChild(element);


        //выводим лого Эталона
        var etalonLogoPath = DirectoryHelper.GetImagesRootPath("Etalon");
        var etalonImage = CreateImage(mainDocument, etalonLogoPath);

        paragraph.AppendChild(etalonImage);

        element = CreateLineBreak();
        paragraph.AppendChild(element);


        //выводим заголовок паспорта
        element = CreateTextLine("Стенд датчиков КИПиА");
        paragraph.AppendChild(element);
        element = CreateLineBreak();
        paragraph.AppendChild(element);

        element = CreateTextLine($"{stand.KKSCode}");
        paragraph.AppendChild(element);
        element = CreateLineBreak();
        paragraph.AppendChild(element);

        element = CreateTextLine("ПАСПОРТ");
        paragraph.AppendChild(element);
        element = CreateLineBreak();
        paragraph.AppendChild(element);


        //вставляем разрыв страницы
        element = CreatePageBreak();
        paragraph.AppendChild(element);

        mainDocument.Document.AppendChild(paragraph);

    }

    public int CreateStandPasportBody(Stand stand, int docStartPage)
    {
        int docPageNumber = docStartPage;



        docPageNumber++;
        return docPageNumber;
    }




    #region Вспомогательные методы

    private void SetLandscapeOrientation(MainDocumentPart mainDocument)
    {
        var sectionProps = new SectionProperties();

        // Размер страницы A4 в ландшафтной ориентации (в EMU)

        var pageSize = new PageSize()
        {
            Width = 16838,
            Height = 11906,
            Orient = PageOrientationValues.Landscape
        };


        // Поля страницы
        PageMargin pageMargin = new PageMargin()
        {
            Top = 1440,       // 2.54 см
            Right = 1440,     // 2.54 см
            Bottom = 1440,    // 2.54 см
            Left = 1440,      // 2.54 см
            Header = 720,     // 1.27 см
            Footer = 720      // 1.27 см
        };


        sectionProps.Append(pageSize);
        mainDocument.Document.Body.Append(sectionProps);
    }


    private Run CreateTextLine(string text)
    {
        return new Run(
            new Text(text));
    }


    private Run CreatePageBreak()
    {
        return new Run(
            new Break() { Type = BreakValues.Page });
    }

    private Run CreateLineBreak()
    {
        return new Run(
               new Break() { Type = BreakValues.TextWrapping });
    }

    private Run CreateImage(MainDocumentPart mainPart, string imagePath)
    {

        if (string.IsNullOrEmpty(imagePath) || !File.Exists(imagePath))
            return null;

        // Определяем тип картинки по расширению
        var ext = Path.GetExtension(imagePath).ToLowerInvariant();
        var partType = ext switch
        {
            ".jpg" or ".jpeg" => ImagePartType.Jpeg,
            ".png" => ImagePartType.Png,
            ".gif" => ImagePartType.Gif,
            ".bmp" => ImagePartType.Bmp,
            ".tiff" or ".tif" => ImagePartType.Tiff,
            _ => throw new InvalidOperationException("Unsupported image type: " + ext)

        };
        ImagePart imagePart = mainPart.AddImagePart(partType);
        using (FileStream stream = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
        {
            imagePart.FeedData(stream);
        }

        string rId = mainPart.GetIdOfPart(imagePart);

        // Размеры картинки в EMU (здесь задаем ширину 200px высоту пропорционально, можно изменить)
        // Для простоты зададим фиксированные размеры
        const long cx = 990000; // приблизительно 1.25" (в EMU)
        const long cy = 792000; // приблизительно 1" (в EMU)

        var element =
            new Drawing(
                new DW.Inline(
                    new DW.Extent { Cx = cx, Cy = cy },
                    new DW.EffectExtent { LeftEdge = 0L, TopEdge = 0L, RightEdge = 0L, BottomEdge = 0L },
                    new DW.DocProperties { Id = (UInt32Value)1U, Name = Path.GetFileName(imagePath) },
                    new DW.NonVisualGraphicFrameDrawingProperties(
                        new A.GraphicFrameLocks { NoChangeAspect = true }),
                    new A.Graphic(
                        new A.GraphicData(
                            new PIC.Picture(
                                new PIC.NonVisualPictureProperties(
                                    new PIC.NonVisualDrawingProperties { Id = (UInt32Value)0U, Name = Path.GetFileName(imagePath) },
                                    new PIC.NonVisualPictureDrawingProperties()),
                                new PIC.BlipFill(
                                    new A.Blip { Embed = rId },
                                    new A.Stretch(new A.FillRectangle())),
                                new PIC.ShapeProperties(
                                    new A.Transform2D(new A.Offset { X = 0L, Y = 0L }, new A.Extents { Cx = cx, Cy = cy }),
                                    new A.PresetGeometry(new A.AdjustValueList()) { Preset = A.ShapeTypeValues.Rectangle })
                            )
                        )
                        { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" }
                    )
                )
                {
                    DistanceFromTop = (UInt32Value)0U,
                    DistanceFromBottom = (UInt32Value)0U,
                    DistanceFromLeft = (UInt32Value)0U,
                    DistanceFromRight = (UInt32Value)0U
                }
            );

        return new Run(element);
    }

    #endregion

}