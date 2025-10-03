using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System.Drawing;
using System.Runtime.Versioning;
using A = DocumentFormat.OpenXml.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;

namespace ReportEngine.Export.Helpers
{
    public static class OpenXmlHelper
    {


        public static Run CreateLineBreak()
        {
            return new Run(
                new Break { Type = BreakValues.TextWrapping });
        }

        public static Run CreateTextLine(string text)
        {
            return new Run(
                new Text(text));
        }

        public static Run CreatePageBreak()
        {
            return new Run(
                new Break { Type = BreakValues.Page });
        }


        public static SectionProperties CreateDefaultPageSettings()
        {

            // Размер страницы A4 в ландшафтной ориентации (в TWIPS)

            var pageSize = new PageSize
            {
                Width = 16838,
                Height = 11906,
                Orient = PageOrientationValues.Landscape
            };


            // Поля страницы
            var pageMargin = new PageMargin
            {
                Top = 1440, // 2.54 см
                Right = 1440, // 2.54 см
                Bottom = 1440, // 2.54 см
                Left = 1440, // 2.54 см
                Header = 720, // 1.27 см
                Footer = 720 // 1.27 см
            };

            return new SectionProperties(pageSize, pageMargin);
        }


        public static bool IsFilePathValid(string path) => (!string.IsNullOrEmpty(path) && File.Exists(path));


        [SupportedOSPlatform("windows")]
        public static Run CreateImage(MainDocumentPart mainPart, string imagePath, double scale = 1.0)
        {

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


            double widthInches = 0.0;
            double heightInches = 0.0;


            // Получаем реальные размеры изображения
            using (var image = new Bitmap(imagePath))
            {
                widthInches = image.Width / image.HorizontalResolution;
                heightInches = image.Height / image.VerticalResolution;
            }





            // Конвертируем в EMU и применяем масштаб
            long cx = (long)(widthInches * 914400 * scale); // 1 дюйм = 914400 EMU
            long cy = (long)(heightInches * 914400 * scale);




            var imagePart = mainPart.AddImagePart(partType);
            using (var stream = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
            {
                imagePart.FeedData(stream);
            }

            var rId = mainPart.GetIdOfPart(imagePart);


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
                                            new PIC.NonVisualDrawingProperties
                                            { Id = (UInt32Value)0U, Name = Path.GetFileName(imagePath) },
                                            new PIC.NonVisualPictureDrawingProperties()),
                                        new PIC.BlipFill(
                                            new A.Blip { Embed = rId },
                                            new A.Stretch(new A.FillRectangle())),
                                        new PIC.ShapeProperties(
                                            new A.Transform2D(new A.Offset { X = 0L, Y = 0L },
                                                new A.Extents { Cx = cx, Cy = cy }),
                                            new A.PresetGeometry(new A.AdjustValueList())
                                            { Preset = A.ShapeTypeValues.Rectangle })
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






        public static Table AddVerticalLineWithTable()
        {

            // Создаем таблицу с двумя колонками и вертикальной границей посередине
            Table table = new Table();

            TableProperties tableProperties = new TableProperties(
                new TableBorders(
                    new InsideVerticalBorder() { Val = BorderValues.Dashed, Size = 12 } // Толщина линии
                ),
                new TableWidth() { Width = "100%", Type = TableWidthUnitValues.Pct }
            );

            TableRow row = new TableRow();

            // Первая колонка
            TableCell cell1 = new TableCell(
                new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Pct, Width = "50%" }),
                new Paragraph(new Run(new Text("Левая часть")))
            );

            // Вторая колонка
            TableCell cell2 = new TableCell(
                new TableCellProperties(new TableCellWidth() { Type = TableWidthUnitValues.Pct, Width = "50%" }),
                new Paragraph(new Run(new Text("Правая часть")))
            );

            row.Append(cell1, cell2);
            table.Append(tableProperties, row);
            return table;

        }

    }
}
