using System.Diagnostics;
using ClosedXML.Excel;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Export.ExcelWork.Enums;
using ReportEngine.Export.ExcelWork.Services.Interfaces;
using ReportEngine.Shared.Config.IniHeleprs;

namespace ReportEngine.Export.ExcelWork.Services;

public class ComponentsListReportGenerator : IReportGenerator
{
    private readonly IProjectInfoRepository _projectInfoRepository;

    public ComponentsListReportGenerator(IProjectInfoRepository projectInfoRepository)
    {
        _projectInfoRepository = projectInfoRepository;
    }

    public ReportType Type => ReportType.ComponentsListReport;

    public async Task GenerateAsync(int projectId)
    {
        var project = await _projectInfoRepository.GetByIdAsync(projectId);

        using (var wb = new XLWorkbook())
        {
            wb.Worksheets.ToList().ForEach(ws => ws.Delete());

            foreach (var stand in project.Stands)
            {
                var ws = wb.Worksheets.Add($"Стенд_{stand.KKSCode}");
                CreateWorksheetTableHeader(ws, stand);
                FillWorksheetTable(ws, stand);


                ws.Cells().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                ws.Cells().Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            }


            var savePath = SettingsManager.GetReportDirectory();
            var fileName = "Ведомость комплектующих___" + DateTime.Now.ToString("dd-MM-yy___HH-mm-ss") + ".xlsx";


            var fullSavePath = Path.Combine(savePath, fileName);

            Debug.WriteLine("Отчёт сохранён: " + fullSavePath);
            wb.SaveAs(fullSavePath);
        }
    }

    private void CreateWorksheetTableHeader(IXLWorksheet ws, Stand stand)
    {
        ws.Cell("B1").Value = $"Код-KKS: {stand.KKSCode}";
        ws.Cell("C1").Value = $"Наименование: {stand.Design}";

        ws.Cell("B2").Value = "Сводная ведомость комплектующих";

        ws.Columns().AdjustToContents();



        ws.Cell("B3").Value = "Наименование";
        ws.Cell("C3").Value = "Ед. изм";
        ws.Cell("D3").Value = "Кол.";




        ws.Columns().AdjustToContents();

        var headerRange = ws.Range("B1:D3");

        headerRange.Style.Border.SetOutsideBorder(XLBorderStyleValues.Medium);
        headerRange.Style.Border.SetInsideBorder(XLBorderStyleValues.Medium);

        headerRange.Style.Font.SetBold();


        ws.Range("B2:D2").Merge();
        ws.Range("C1:D1").Merge();
    }



    private void FillWorksheetTable(IXLWorksheet ws, Stand stand)
    {
        var activeRow = 4;


        //Формирование списка труб
        activeRow = CreateSubheaderOnWorksheet(activeRow, "Трубы", ws);
 


        var standPipes = stand.ObvyazkiInStand
            .Select(obv => new
            {
                name = obv.MaterialLine,
                length = obv.MaterialLineCount
            })
            .GroupBy(pipe => pipe.name)
            .Select(group => new
            {
                Name = group.Key,
                Unit = "м", //на единицы измерения из базы
                LengthSum = group.Sum(pipe => pipe.length)
            });


        var pipesRecords = standPipes
            .Select(pipe => (name:pipe.Name, unit:pipe.Unit, quantity:pipe.LengthSum ?? 0f))
            .ToList();





        activeRow = FillSubtableData(activeRow, pipesRecords, ws);
      








        //Формирование списка арматуры
        CreateSubheaderOnWorksheet(activeRow, "Арматура", ws);
        activeRow++;


        var armatures = stand.ObvyazkiInStand
            .Select(obv => new
            {
                armName = obv.Armature,
                armQuantity = obv.ArmatureCount
            })
            .GroupBy(obv => obv.armName)
            .Select(group => new
            {
                name = group.Key,
                unit = "шт", //на единицы измерения из базы
                quantity = group.Sum(arm => arm.armQuantity)
            });



        foreach (var arm in armatures)
        {
            ws.Cell($"B{activeRow}").Value = arm.name;
            ws.Cell($"C{activeRow}").Value = arm.unit; ;
            ws.Cell($"D{activeRow}").Value = arm.lengthSum;
            activeRow++;
        }


        //Формирование списка тройников и КМЧ
        CreateSubheaderOnWorksheet(activeRow, "Тройники и КМЧ", ws);

        


    }


    //создает заголовок для подтаблицы и возвращает следующую строку
    int CreateSubheaderOnWorksheet(int row, string title, IXLWorksheet ws)
    {
        var subHeaderRange = ws.Range($"A{row}:D{row}");
        subHeaderRange.Merge();
        subHeaderRange.Value = title;
        subHeaderRange.Style.Font.SetFontSize(10);
        subHeaderRange.Style.Font.SetBold();
        row++;
        return row;
    }

    //Заполняет подтаблицу и возвращает следующую строку
    int FillSubtableData(int startRow, List<(string name, string unit, float quantity)> items, IXLWorksheet ws)
    {
        int currentRow = startRow;
        foreach (var item in items)
        {
            ws.Cell($"B{currentRow}").Value = item.name;
            ws.Cell($"C{currentRow}").Value = item.unit;
            ws.Cell($"D{currentRow}").Value = item.quantity;
            currentRow++;
        } 
        return currentRow;
    }
}