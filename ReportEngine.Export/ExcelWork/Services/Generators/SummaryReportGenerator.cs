using ClosedXML.Excel;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Export.ExcelWork.Enums;
using ReportEngine.Export.ExcelWork.Services.Generators.DTO;
using ReportEngine.Export.ExcelWork.Services.Interfaces;
using ReportEngine.Export.Mapping;
using ReportEngine.Shared.Config.IniHeleprs;
using ReportEngine.Shared.Config.IniHelpers;
using ReportEngine.Shared.Config.IniHelpers.CalculationSettings;
using ReportEngine.Shared.Config.IniHelpers.CalculationSettingsData;
using System.Diagnostics;

namespace ReportEngine.Export.ExcelWork.Services.Generators;

public class SummaryReportGenerator : IReportGenerator
{
    private readonly IProjectInfoRepository _projectInfoRepository;

    public SummaryReportGenerator(IProjectInfoRepository projectInfoRepository)
    {
        _projectInfoRepository = projectInfoRepository;
    }


    public ReportType Type => ReportType.SummaryReport;


    public async Task GenerateAsync(int projectId)
    {
        var project = await _projectInfoRepository.GetByIdAsync(projectId);

        using (var wb = new XLWorkbook())
        {
            var standNumber = 1;

            //заполняем листы по стендам
            foreach (var stand in project.Stands)
            {
                var ws = wb.Worksheets.Add($"{standNumber}");

                CreateStandTableHeader(ws, stand, XLAlignmentHorizontalValues.Center);
                FillStandTable(ws, stand);

                ws.Columns().AdjustToContents();
                ws.Rows().AdjustToContents();

                standNumber++;
            }


            //заполняем сводную ведомость
            var summarySheet = wb.Worksheets.Add("Сводная заявка");

            CreateCommonListTableHeader(summarySheet, project);
            FillCommonListTable(summarySheet, project);


            //заполняем калькуляцию
            var calculationSheet = wb.Worksheets.Add("Калькуляция");

            CreateCalcullationTableHeader(calculationSheet, project);
            FillCalculationTable(calculationSheet, project);


            //применяем оформление ко всему документу
            foreach (var ws in wb.Worksheets)
            {
                ws.Cells().Style.Font.FontName = "Times New Roman";
                ws.Cells().Style.Alignment.WrapText = true;
                ws.Columns().AdjustToContents();
                ws.Rows().AdjustToContents();
            }

            var savePath = SettingsManager.GetReportDirectory();
            var fileName = ExcelReportHelper.CreateReportName("Сводная_ведомость", "xlsx");
            var fullSavePath = Path.Combine(savePath, fileName);

            Debug.WriteLine("Отчёт сохранён: " + fullSavePath);
            wb.SaveAs(fullSavePath);
        }
    }


    #region Вспомогательные

    private float? TryToParseFloat(string str)
    {
        return float.TryParse(str, out float parseResult) ? parseResult : null;
    }

    private SummaryReportStandsData GenerateStandsData(IEnumerable<Stand> stands)
    {
        const string dbErrorString = "Ошибка получения данных из БД";

        //Формирование списка труб
        var pipesList = stands
            .SelectMany(stand => stand.ObvyazkiInStand)
            .Select(obv => new
            {
                name = obv.MaterialLine,
                units = obv.MaterialLineMeasure,
                length = obv.MaterialLineCount,
                price = obv.MaterialLineCostPerUnit
            })
            .GroupBy(pipe => pipe.name)
            .Select(group => new
            {
                name = group.Key,
                unit = group.First().units,
                quantity = group.Sum(pipe => pipe.length),
                costPerUnit = group.First().price
            })
            .Select(group => new ReportStandData
            {
                Name = group.name ?? dbErrorString,
                Unit = group.unit ?? dbErrorString,
                Quantity = group.quantity.ToString() ?? dbErrorString,
                CostPerUnit = group.costPerUnit ?? dbErrorString,
                CommonCost = group.costPerUnit == null || group.quantity == null
                    ? dbErrorString
                    : (float.Parse(group.costPerUnit) * group.quantity).ToString()
            })
            .ToList();


        //Формирование списка арматуры
        var armaturesList = stands
            .SelectMany(stand => stand.ObvyazkiInStand)
            .Select(obv => new
            {
                name = obv.Armature,
                units = obv.ArmatureMeasure,
                quantity = obv.ArmatureCount,
                price = obv.ArmatureCostPerUnit
            })
            .GroupBy(arm => arm.name)
            .Select(group => new
            {
                name = group.Key,
                unit = group.First().units,
                quantity = group.Sum(arm => arm.quantity),
                costPerUnit = group.First().price
            })
            .Select(group => new ReportStandData
            {
                Name = group.name ?? dbErrorString,
                Unit = group.unit ?? dbErrorString,
                Quantity = group.quantity.ToString() ?? dbErrorString,
                CostPerUnit = group.costPerUnit ?? dbErrorString,
                CommonCost = group.costPerUnit == null || group.quantity == null
                    ? dbErrorString
                    : (float.Parse(group.costPerUnit) * group.quantity).ToString()
            })
            .ToList();


        //Формирование списка тройников и КМЧ
        var treeList = stands
            .SelectMany(stand => stand.ObvyazkiInStand)
            .Select(obv => new
            {
                name = obv.TreeSocket,
                units = obv.TreeSocketMaterialMeasure,
                quantity = obv.TreeSocketCount,
                price = obv.TreeSocketMaterialCostPerUnit
            })
            .GroupBy(item => item.name)
            .Select(group => new
            {
                name = group.Key,
                unit = group.First().units,
                quantity = group.Sum(tree => tree.quantity),
                costPerUnit = group.First().price
            })
            .Select(group => new ReportStandData
            {
                Name = group.name ?? dbErrorString,
                Unit = group.unit ?? dbErrorString,
                Quantity = group.quantity.ToString() ?? dbErrorString,
                CostPerUnit = group.costPerUnit ?? dbErrorString,
                CommonCost = group.costPerUnit == null || group.quantity == null
                    ? dbErrorString
                    : (float.Parse(group.costPerUnit) * group.quantity).ToString()
            })
            .ToList();


        var kmchList = stands
            .SelectMany(stand => stand.ObvyazkiInStand)
            .Select(obv => new
            {
                name = obv.KMCH,
                units = obv.KMCHMeasure,
                quantity = obv.KMCHCount,
                price = obv.KMCHCostPerUnit
            })
            .GroupBy(item => item.name)
            .Select(group => new
            {
                name = group.Key,
                unit = group.First().units,
                quantity = group.Sum(tree => tree.quantity),
                costPerUnit = group.First().price
            })
            .Select(group => new ReportStandData
            {
                Name = group.name ?? dbErrorString,
                Unit = group.unit ?? dbErrorString,
                Quantity = group.quantity.ToString() ?? dbErrorString,
                CostPerUnit = group.costPerUnit ?? dbErrorString,
                CommonCost = group.costPerUnit == null || group.quantity == null
                    ? dbErrorString
                    : (float.Parse(group.costPerUnit) * group.quantity).ToString()
            })
            .ToList();


        //формирование дренажа
        var drainageParts = stands
            .SelectMany(stand => stand.StandDrainages)
            .SelectMany(drainage => drainage.Drainage.Purposes)
            .GroupBy(purpose => purpose.Material)
            .Select(group => new
            {
                name = group.Key,
                unit = group.First().Measure,
                quantity = group.Sum(groupElement => groupElement.Quantity),
                costPerUnit = group.First().CostPerUnit
            })
            .Select(group => new ReportStandData
            {
                Name = group.name ?? dbErrorString,
                Unit = group.unit ?? dbErrorString,
                Quantity = group.quantity.ToString() ?? dbErrorString,
                CostPerUnit = group.costPerUnit.ToString() ?? dbErrorString,
                CommonCost = group.costPerUnit == null || group.quantity == null
                    ? dbErrorString
                    : (group.costPerUnit * group.quantity).ToString()
            })
            .ToList();


        //Формирование списка рамных комплектующих
        var framesList = stands
            .SelectMany(stand => stand.StandFrames)
            .SelectMany(fr => fr.Frame.Components)
            .Select(comp => new
            {
                name = comp.ComponentName,
                unit = comp.Measure,
                quantity = comp.Count,
                costPerUnit = comp.CostComponent
            })
            .GroupBy(frameComp => frameComp.name)
            .Select(group => new
            {
                name = group.Key,
                group.First().unit,
                quantity = group.Sum(frameComp => frameComp.quantity),
                group.First().costPerUnit
            })
            .Select(record => new ReportStandData
            {
                Name = record.name ?? dbErrorString,
                Unit = record.unit ?? dbErrorString,
                Quantity = record.quantity.ToString() ?? dbErrorString,
                CostPerUnit = record.costPerUnit.ToString() ?? dbErrorString,
                CommonCost = record.costPerUnit == null || record.quantity == null
                    ? dbErrorString
                    : (record.costPerUnit * record.quantity).ToString()
            })
            .ToList();


        //формирование списка кронштейнов
        var sensorsHolders = stands
            .SelectMany(stand => stand.StandAdditionalEquips)
            .SelectMany(equip => equip.AdditionalEquip.Purposes)
            .Where(purpose =>
                purpose.Purpose.Contains("Кронштейн")) //сомнительная хрень, хз что брать за источник информации
            .GroupBy(purpose => purpose.Material)
            .Select(group => new
            {
                name = group.Key,
                unit = group.First().Measure,
                quantity = group.Sum(groupElement => groupElement.Quantity),
                costPerUnit = group.First().CostPerUnit
            })
            .Select(record => new ReportStandData
            {
                Name = record.name ?? dbErrorString,
                Unit = record.unit ?? dbErrorString,
                Quantity = record.quantity.ToString() ?? dbErrorString,
                CostPerUnit = record.costPerUnit.ToString() ?? dbErrorString,
                CommonCost = record.costPerUnit == null || record.quantity == null
                    ? dbErrorString
                    : (record.costPerUnit * record.quantity).ToString()
            })
            .ToList();


        //формирование списка электрических комплектующих
        var electricalParts = stands
            .SelectMany(stand => stand.StandElectricalComponent)
            .SelectMany(equip => equip.ElectricalComponent.Purposes)
            .GroupBy(purpose => purpose.Material)
            .Select(group => new
            {
                name = group.Key,
                unit = group.First().Measure,
                quantity = group.Sum(item => item.Quantity),
                costPerUnit = group.First().CostPerUnit
            })
            .Select(record => new ReportStandData
            {
                Name = record.name ?? dbErrorString,
                Unit = record.unit ?? dbErrorString,
                Quantity = record.quantity.ToString() ?? dbErrorString,
                CostPerUnit = record.costPerUnit.ToString() ?? dbErrorString,
                CommonCost = record.costPerUnit == null || record.quantity == null
                    ? dbErrorString
                    : (record.costPerUnit * record.quantity).ToString()
            })
            .ToList();


        //формирование списка дополнительного оборудования
        var additionalParts = stands
            .SelectMany(stand => stand.StandAdditionalEquips)
            .SelectMany(equip => equip.AdditionalEquip.Purposes)
            .GroupBy(purpose => purpose.Material)
            .Select(group => new
            {
                name = group.Key,
                unit = group.First().Measure,
                quantity = group.Sum(item => item.Quantity),
                costPerUnit = group.First().CostPerUnit
            })
            .Select(record => new ReportStandData
            {
                Name = record.name ?? dbErrorString,
                Unit = record.unit ?? dbErrorString,
                Quantity = record.quantity.ToString() ?? dbErrorString,
                CostPerUnit = record.costPerUnit.ToString() ?? dbErrorString,
                CommonCost = record.costPerUnit == null || record.quantity == null
                    ? dbErrorString
                    : (record.costPerUnit * record.quantity).ToString()
            })
            .Except(sensorsHolders);

        var othersParts = additionalParts
            .Where(part => part.Name.Contains("Шильдик") || part.Name.Contains("Табличка")) //тоже сомнительно
            .ToList();

        var supplies = additionalParts
            .Except(othersParts)
            .ToList();


        return new SummaryReportStandsData(
            pipesList,
            armaturesList,
            treeList,
            kmchList,
            drainageParts,
            framesList,
            sensorsHolders,
            electricalParts,
            othersParts,
            supplies
        );
    }

    private SummaryReportLaborData GenerateLaborData(IEnumerable<Stand> stands)
    {
        const string settingsErrorString = "Ошибка получения настроек расчета";
        const string dbErrorString = "Ошибка получения данных из БД";

        var frameSettings = CalculationSettingsManager.Load<FrameSettings, FrameSettingsData>();
        var electicalSettings = CalculationSettingsManager.Load<ElectricalSettings, ElectricalSettingsData>();
        var humanCostSettings = CalculationSettingsManager.Load<HumanCostSettings, HumanCostSettingsData>();
        var standSettings = CalculationSettingsManager.Load<StandSettings, StandSettingsData>();
        var sandblastSettings = CalculationSettingsManager.Load<SandBlastSettings, SandBlastSettingsData>();




        //трудозатраты на изготовление
        var frameProductionHumanCostSum = stands
            .Select(_ => frameSettings?.TimeForProductionFrame)
            .Aggregate((thisTimeCost, nextTimeCost) => thisTimeCost + nextTimeCost);

        var frameProductionRecord = new ReportStandData
        {
            Name = "Изготовление рам",
            Unit = "чел/час",
            Quantity = frameProductionHumanCostSum?.ToString() ?? settingsErrorString,
            CostPerUnit = frameSettings?.FrameProduction.ToString() ?? settingsErrorString,
            CommonCost = (frameProductionHumanCostSum * frameSettings?.FrameProduction).ToString() ?? settingsErrorString
        };





        

        foreach (var stand in stands)
        {

            //трудозатраты на обвязки
            var allObvHumanCosts = stands
                .SelectMany(stand => stand.ObvyazkiInStand)
                .Select(obv => obv.HumanCost);

            var invalidQuantityData = allObvHumanCosts
                .Any(cost => cost == null);

            var humanCostsSum = allObvHumanCosts
                .Where(cost => cost != null)
                .Sum();



            //формируем строчку для количества
            string resultQuantitySumString = humanCostsSum.ToString() + "\n";

            if (invalidQuantityData)
            {
                resultQuantitySumString += dbErrorString + "\n";
            }



            //формируем строчку для общих затрат
            var resultCommonCost = humanCostsSum * (humanCostSettings?.ObvzyakaProduction ?? 0.0f);

            string resultCommonCostString = resultCommonCost.ToString() + "\n";

            if (invalidQuantityData)
            {
                resultCommonCostString += dbErrorString + "\n";
            }

            if (humanCostSettings == null)
            {
                resultCommonCostString += settingsErrorString + "\n";
            }

            var obvProductionRecord = new ReportStandData
            {
                Name = "Изготовление обвязок",
                Unit = "чел/час",
                Quantity = resultQuantitySumString,
                CostPerUnit = humanCostSettings?.ObvzyakaProduction.ToString() ?? settingsErrorString,
                CommonCost = resultCommonCostString
            };


            //трудозатраты на коллектор
            var collectorProductionRecord = new ReportStandData
            {
                Name = "Изготовление коллектора",
                Unit = "чел/час",
                Quantity = humanCostSettings?.TimeForCollectorBoil.ToString() ?? settingsErrorString,
                CostPerUnit = humanCostSettings?.CollectorProduction.ToString() ?? settingsErrorString,
                CommonCost = (humanCostSettings?.TimeForCollectorBoil * humanCostSettings?.CollectorProduction).ToString() ?? settingsErrorString
            };


            //трудозатраты на испытания
            var qualityTestRecord = new ReportStandData
            {
                Name = "Испытание на прочность и герметичность",
                Unit = "чел/час",
                Quantity = "N/A",
                CostPerUnit = "N/A",
                CommonCost = "N/A"
            };

            //трудозатраты на пескоструйные работы
            var sandblastingRecord = new ReportStandData
            {
                Name = "Пескоструйные работы",
                Unit = "чел/час",
                Quantity = "N/A",
                CostPerUnit = "N/A",
                CommonCost = "N/A"
            };


            //трудозатраты на покраску
            var paintingRecord = new ReportStandData
            {
                Name = "Покраска",
                Unit = "чел/час",
                Quantity = "N/A",
                CostPerUnit = "N/A",
                CommonCost = "N/A"
            };

            //трудозатраты на электромонтаж
            var electricRecord = new ReportStandData
            {
                Name = "Электромонтаж",
                Unit = "чел/час",
                Quantity = "N/A",
                CostPerUnit = "N/A",
                CommonCost = "N/A"
            };

            //трудозатраты на общую проверку стенда
            var commonCheckRecord = new ReportStandData
            {
                Name = "Общая проверка стенда",
                Unit = "чел/час",
                Quantity = "N/A",
                CostPerUnit = "N/A",
                CommonCost = "N/A"
            };
        }

        return new SummaryReportLaborData(
      frameProductionRecord,
      obvProductionRecord,
      collectorProductionRecord,
      qualityTestRecord,
      sandblastingRecord,
      paintingRecord,
      electricRecord,
      commonCheckRecord);
    }

    #endregion


    #region Элементы таблицы

    //создает подзаголовок для подтаблицы и возвращает следующую строку
    private int CreateSubheaderOnWorksheet(int row, string title, IXLWorksheet ws)
    {
        var subHeaderRange = ws.Range($"B{row}:F{row}");
        subHeaderRange.Merge();
        subHeaderRange.Value = title;
        subHeaderRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        subHeaderRange.Style.Font.SetFontSize(10);
        subHeaderRange.Style.Font.SetBold();

        row++;
        return row;
    }

    //создает заголовок для стенда
    private void CreateStandTableHeader(IXLWorksheet ws, Stand stand, XLAlignmentHorizontalValues alignment)
    {
        var headerRange = ws.Range("A1:F3");


        var exportDaysRange = ws.Range("A2:A3").Merge();
        exportDaysRange.Value = "Срок поставки комплектующих, дней";

        var kksCodeRange = ws.Range("C1:F1").Merge();
        kksCodeRange.Value = $"Код KKS:{stand.KKSCode}";

        var summaryComponentsList = ws.Range("B2:F2").Merge();
        summaryComponentsList.Value = "Сводная ведомость комплектующих";


        ws.Cell("B3").Value = "Наименование";
        ws.Cell("C3").Value = "Ед.изм.";
        ws.Cell("D3").Value = "Кол.";
        ws.Cell("E3").Value = "Цена за шт., руб";
        ws.Cell("F3").Value = "Цена, руб";


        headerRange.Style.Font.SetBold();
        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
        headerRange.Style.Border.SetInsideBorder(XLBorderStyleValues.Medium);
        headerRange.Style.Border.SetOutsideBorder(XLBorderStyleValues.Medium);
    }

    //создает заголовок сводной ведомости
    private void CreateCommonListTableHeader(IXLWorksheet ws, ProjectInfo project)
    {
        var headerRange = ws.Range("B1:F3");

        var customerCompanyRange = ws.Range("B1:F1").Merge();
        customerCompanyRange.Value = $"{project.Company}";
        customerCompanyRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        var commonListStringRange = ws.Range("B2:F2").Merge();
        commonListStringRange.Value = "Сводная ведомость комплектующих";
        commonListStringRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        ws.Cell("B3").Value = "Наименование";
        ws.Cell("C3").Value = "Ед.изм.";
        ws.Cell("D3").Value = "Кол.";
        ws.Cell("E3").Value = "Цена за шт., руб";
        ws.Cell("F3").Value = "Цена, руб";

        ws.Columns().AdjustToContents();

        headerRange.Style.Border.SetOutsideBorder(XLBorderStyleValues.Medium);
        headerRange.Style.Border.SetInsideBorder(XLBorderStyleValues.Medium);
        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

        headerRange.Style.Font.SetBold();
    }

    //создает заголовок листа калькуляции
    private void CreateCalcullationTableHeader(IXLWorksheet ws, ProjectInfo project)
    {
        var headerRange = ws.Range("A1:K6");


        var customerCompanyRange = ws.Range("C1:K2").Merge();
        customerCompanyRange.Value = $"{project.Company}";

        var descriptionRange = ws.Range("A3:A6").Merge();
        descriptionRange.Value = "Примечание";

        var deliveryTimeRange = ws.Range("B3:B6").Merge();
        deliveryTimeRange.Value = "Срок поставки комплектующих, дней";

        var devicesListRange = ws.Range("C3:K4").Merge();
        devicesListRange.Value = "Перечень оборудования, планируемого к поставке";

        var numberRange = ws.Range("C5:C6").Merge();
        numberRange.Value = "№ п/п";

        var standNameRange = ws.Range("D5:D6").Merge();
        standNameRange.Value = "Наименование стенда";

        var kksCodeRange = ws.Range("E5:E6").Merge();
        kksCodeRange.Value = "Код-KKS";

        var unitsRange = ws.Range("F5:F6").Merge();
        unitsRange.Value = "Ед.изм.";

        var quantityRange = ws.Range("G5:G6").Merge();
        quantityRange.Value = "Кол-во";

        var massRange = ws.Range("H5:H6").Merge();
        massRange.Value = "Масса, кг";

        var widthRange = ws.Range("I5:I6").Merge();
        widthRange.Value = "Ширина стенда, мм";

        var costPerUnitRange = ws.Range("J5:J6").Merge();
        costPerUnitRange.Value = "Цена за единицу, руб.";

        var priceRange = ws.Range("K5:K6").Merge();
        priceRange.Value = "Стоимость, руб";


        headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

        ws.Range("A3:K6").Style.Border.SetOutsideBorder(XLBorderStyleValues.Medium);
        ws.Range("A3:K6").Style.Border.SetInsideBorder(XLBorderStyleValues.Medium);

        customerCompanyRange.Style.Border.SetOutsideBorder(XLBorderStyleValues.Medium);
        customerCompanyRange.Style.Border.SetInsideBorder(XLBorderStyleValues.Medium);

        headerRange.Style.Font.SetBold();

    }



    #endregion



    #region Заполнители

    //заполняет целиком таблицу для стенда
    private void FillStandTable(IXLWorksheet ws, Stand stand)
    {
        var activeRow = 4;

        var generatedData = GenerateStandsData(new List<Stand> { stand });


        activeRow = CreateSubheaderOnWorksheet(activeRow, "Сортамент труб", ws);
        activeRow = FillSubtableData(activeRow, generatedData.PipesList, ws);
        activeRow = CreateGroupTotalRecord(activeRow, generatedData.PipesList, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Арматура", ws);
        activeRow = FillSubtableData(activeRow, generatedData.ArmaturesList, ws);
        activeRow = CreateGroupTotalRecord(activeRow, generatedData.ArmaturesList, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Тройники и КМЧ", ws);
        activeRow = FillSubtableData(activeRow, generatedData.TreeList, ws);
        activeRow = FillSubtableData(activeRow, generatedData.KmchList, ws);

        //общий список, чтобы запихнуть в метод
        var treeAndKmchList = new List<ReportStandData>();
        treeAndKmchList.AddRange(generatedData.TreeList);
        treeAndKmchList.AddRange(generatedData.KmchList);

        activeRow = CreateGroupTotalRecord(activeRow, treeAndKmchList, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Дренаж", ws);
        activeRow = FillSubtableData(activeRow, generatedData.DrainageParts, ws);
        activeRow = CreateGroupTotalRecord(activeRow, generatedData.DrainageParts, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Рамные комплектующие", ws);
        activeRow = FillSubtableData(activeRow, generatedData.FramesList, ws);
        activeRow = CreateGroupTotalRecord(activeRow, generatedData.FramesList, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Кронштейны", ws);
        activeRow = FillSubtableData(activeRow, generatedData.SensorsHolders, ws);
        activeRow = CreateGroupTotalRecord(activeRow, generatedData.SensorsHolders, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Электрические компоненты", ws);
        activeRow = FillSubtableData(activeRow, generatedData.ElectricalParts, ws);
        activeRow = CreateGroupTotalRecord(activeRow, generatedData.ElectricalParts, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Прочие", ws);
        activeRow = FillSubtableData(activeRow, generatedData.OthersParts, ws);
        activeRow = CreateGroupTotalRecord(activeRow, generatedData.OthersParts, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Расходные материалы", ws);
        activeRow = FillSubtableData(activeRow, generatedData.Supplies, ws);
        activeRow = CreateGroupTotalRecord(activeRow, generatedData.Supplies, ws);

        activeRow = CreatePartsTotalRecord(activeRow, generatedData, ws);

    }

    //Заполняет подтаблицу и возвращает следующую строку
    private int FillSubtableData(int startRow, List<ReportStandData> items, IXLWorksheet ws)
    {
        var currentRow = startRow;

        foreach (var item in items)
        {
            ws.Cell($"B{currentRow}").Value = item.Name;
            ws.Cell($"C{currentRow}").Value = item.Unit;
            ws.Cell($"D{currentRow}").Value = item.Quantity;
            ws.Cell($"E{currentRow}").Value = item.CostPerUnit;
            ws.Cell($"F{currentRow}").Value = item.CommonCost;

            ws.Range($"B{currentRow}:F{currentRow}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            currentRow++;
        }

        return currentRow;
    }

    //заполняет сводную ведомость
    private void FillCommonListTable(IXLWorksheet ws, ProjectInfo project)
    {
        var generatedData = GenerateStandsData(project.Stands);

        var activeRow = 4;

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Сортамент труб", ws);
        activeRow = FillSubtableData(activeRow, generatedData.PipesList, ws);
        activeRow = CreateGroupTotalRecord(activeRow, generatedData.PipesList, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Арматура", ws);
        activeRow = FillSubtableData(activeRow, generatedData.ArmaturesList, ws);
        activeRow = CreateGroupTotalRecord(activeRow, generatedData.ArmaturesList, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Тройники и КМЧ", ws);
        activeRow = FillSubtableData(activeRow, generatedData.TreeList, ws);
        activeRow = FillSubtableData(activeRow, generatedData.KmchList, ws);

        //общий список, чтобы запихнуть в метод
        var treeAndKmchList = new List<ReportStandData>();
        treeAndKmchList.AddRange(generatedData.TreeList);
        treeAndKmchList.AddRange(generatedData.KmchList);

        activeRow = CreateGroupTotalRecord(activeRow, treeAndKmchList, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Дренаж", ws);
        activeRow = FillSubtableData(activeRow, generatedData.DrainageParts, ws);
        activeRow = CreateGroupTotalRecord(activeRow, generatedData.DrainageParts, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Рамные комплектующие", ws);
        activeRow = FillSubtableData(activeRow, generatedData.FramesList, ws);
        activeRow = CreateGroupTotalRecord(activeRow, generatedData.FramesList, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Кронштейны", ws);
        activeRow = FillSubtableData(activeRow, generatedData.SensorsHolders, ws);
        activeRow = CreateGroupTotalRecord(activeRow, generatedData.SensorsHolders, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Электрические компоненты", ws);
        activeRow = FillSubtableData(activeRow, generatedData.ElectricalParts, ws);
        activeRow = CreateGroupTotalRecord(activeRow, generatedData.ElectricalParts, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Прочие", ws);
        activeRow = FillSubtableData(activeRow, generatedData.OthersParts, ws);
        activeRow = CreateGroupTotalRecord(activeRow, generatedData.OthersParts, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Расходные материалы", ws);
        activeRow = FillSubtableData(activeRow, generatedData.Supplies, ws);
        activeRow = CreateGroupTotalRecord(activeRow, generatedData.Supplies, ws);

        activeRow = CreatePartsTotalRecord(activeRow, generatedData, ws);
    }

    //заполняет лист калькуляции
    private void FillCalculationTable(IXLWorksheet ws, ProjectInfo project)
    {

    }

    //создает записи трудозатрат
    private int FillLaborCostSubtable(int startRow, IXLWorksheet ws, SummaryReportLaborData laborCostData)
    {
        var activeRow = startRow;



        return activeRow;

    }

    #endregion



    #region Итоговые

    //TODO: переделать эту хуйню
    private int CreateGroupTotalRecord(int row, List<ReportStandData> equipmentList, IXLWorksheet ws)
    {

        var activeRow = row;

        ws.Cell($"B{activeRow}").Value = "Итого по категории:";
        ws.Cell($"B{activeRow}").Style.Font.SetBold();
        ws.Cell($"B{activeRow}").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);


        //парсим все в nullable float
        var parsedCosts = equipmentList
            .Select(equipment => TryToParseFloat(equipment.CommonCost));



        string resultSumString = "";

        //проверка на элементы null в списке
        if (parsedCosts.Any(cost => cost == null))
        {
            resultSumString = parsedCosts.Where(cost => cost != null).Sum().ToString() + "\n";
            resultSumString += "В БД отсутствуют необходимые значения для суммирования \n";
            resultSumString += "Результат может быть недостоверным";
        }
        else
        {
            resultSumString = parsedCosts.Sum().ToString();
        }

        ws.Cell($"F{activeRow}").Value = resultSumString;
        ws.Cell($"F{activeRow}").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

        activeRow++;
        return activeRow;
    }

    private int CreatePartsTotalRecord(int row, SummaryReportStandsData partsInfo, IXLWorksheet ws)
    {

        var activeRow = row;

        ws.Cell($"B{activeRow}").Value = "Итого по комплектующим:";
        ws.Cell($"B{activeRow}").Style.Font.SetBold();
        ws.Cell($"B{activeRow}").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);


        //парсим все в nullable float
        // var parsedCosts = partsInfo





        var allCosts = new List<float?>();

        var recordProperties = partsInfo.GetType().GetProperties();

        //обрабатываем все стоимости из каждого перечня
        foreach (var recordProperty in recordProperties)
        {
            var partsListObject = recordProperty.GetValue(partsInfo);
            var partsList = partsListObject as List<ReportStandData>;

            var partsCosts = partsList.Select(part => TryToParseFloat(part.CommonCost));

            allCosts.AddRange(partsCosts);
        }

        string resultSumString = "";

        ////проверка на элементы null в списке
        if (allCosts.Any(cost => cost == null))
        {
            resultSumString = allCosts.Where(cost => cost != null).Sum().ToString() + "\n";
            resultSumString += "В БД отсутствуют необходимые значения для суммирования \n";
            resultSumString += "Результат может быть недостоверным";
        }
        else
        {
            resultSumString = allCosts.Sum().ToString();
        }

        ws.Cell($"F{activeRow}").Value = resultSumString;
        ws.Cell($"F{activeRow}").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);

        activeRow++;
        return activeRow;
    }



    #endregion
}