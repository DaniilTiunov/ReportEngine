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
using System.Collections.Generic;
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

    //создаем инфу о комплектующих
    private SummaryReportStandsData GeneratePartsData(IEnumerable<Stand> stands)
    {
        //Формирование списка труб
        var pipesList = stands
            .SelectMany(stand => stand.ObvyazkiInStand)
            .Select(obv => new
            {
                name = obv.MaterialLine,
                units = obv.MaterialLineMeasure,
                length = obv.MaterialLineCount,
                price = obv.MaterialLineCostPerUnit,
                exportDays = obv.MaterialLineExportDays
            })
            .GroupBy(pipe => pipe.name)
            .Select(group => new
            {
                name = group.Key,
                unit = group.FirstOrDefault().units,
                quantity = group.Sum(pipe => pipe.length),
                costPerUnit = group.FirstOrDefault().price,
                exportDays = group.FirstOrDefault().exportDays
            })
            .Select(group => new ReportRecordData
            {
                ExportDays = new ValidatedField<int?>(group.exportDays, group.exportDays.HasValue),
                Name = new ValidatedField<string?>(group.name, group.name != null),
                Unit = new ValidatedField<string?>(group.unit, group.unit != null),
                Quantity = new ValidatedField<float?>(group.quantity, group.quantity != null),
                CostPerUnit = new ValidatedField<float?>(ExcelReportHelper.TryToParseFloat(group.costPerUnit), group.costPerUnit != null),
            })
            .Select(record =>
            {
                record.CommonCost = new ValidatedField<float?>(
                   record.Quantity.Value * record.CostPerUnit.Value,
                   (record.Quantity.Value * record.CostPerUnit.Value) != null);

                return record;
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
                price = obv.ArmatureCostPerUnit,
                exportDays = obv.MaterialLineExportDays
            })
            .GroupBy(arm => arm.name)
            .Select(group => new
            {
                name = group.Key,
                unit = group.First().units,
                quantity = group.Sum(arm => arm.quantity),
                costPerUnit = group.First().price,
                exportDays = group.First().exportDays

            })
            .Select(group => new ReportRecordData
            {
                 ExportDays = new ValidatedField<int?>(group.exportDays, group.exportDays.HasValue),
                 Name = new ValidatedField<string?>(group.name, group.name != null),
                 Unit = new ValidatedField<string?>(group.unit, group.unit != null),
                 Quantity = new ValidatedField<float?>(group.quantity, group.quantity != null),
                 CostPerUnit = new ValidatedField<float?>(ExcelReportHelper.TryToParseFloat(group.costPerUnit), group.costPerUnit != null),
            })
            .Select(record =>
            {
                record.CommonCost = new ValidatedField<float?>(
                   record.Quantity.Value * record.CostPerUnit.Value,
                   (record.Quantity.Value * record.CostPerUnit.Value) != null);

                return record;
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
                price = obv.TreeSocketMaterialCostPerUnit,
                exportDays = obv.MaterialLineExportDays
            })
            .GroupBy(item => item.name)
            .Select(group => new
            {
                name = group.Key,
                unit = group.First().units,
                quantity = group.Sum(tree => tree.quantity),
                costPerUnit = group.First().price,
                exportDays = group.First().exportDays
            })
           .Select(group => new ReportRecordData
            {
               ExportDays = new ValidatedField<int?>(group.exportDays, group.exportDays.HasValue),
               Name = new ValidatedField<string?>(group.name, group.name != null),
               Unit = new ValidatedField<string?>(group.unit, group.unit != null),
               Quantity = new ValidatedField<float?>(group.quantity, group.quantity != null),
               CostPerUnit = new ValidatedField<float?>(ExcelReportHelper.TryToParseFloat(group.costPerUnit), group.costPerUnit != null),
            })
            .Select(record =>
            {
                record.CommonCost = new ValidatedField<float?>(
                   record.Quantity.Value * record.CostPerUnit.Value,
                   (record.Quantity.Value * record.CostPerUnit.Value) != null);

                return record;
            })
            .ToList();


        var kmchList = stands
            .SelectMany(stand => stand.ObvyazkiInStand)
            .Select(obv => new
            {
                name = obv.KMCH,
                units = obv.KMCHMeasure,
                quantity = obv.KMCHCount,
                price = obv.KMCHCostPerUnit,
                exportDays = obv.MaterialLineExportDays
            })
            .GroupBy(item => item.name)
            .Select(group => new
            {
                name = group.Key,
                unit = group.First().units,
                quantity = group.Sum(tree => tree.quantity),
                costPerUnit = group.First().price,
                exportDays = group.First().exportDays
            })
           .Select(group => new ReportRecordData
           {
               ExportDays = new ValidatedField<int?>(group.exportDays, group.exportDays.HasValue),
               Name = new ValidatedField<string?>(group.name, group.name != null),
               Unit = new ValidatedField<string?>(group.unit, group.unit != null),
               Quantity = new ValidatedField<float?>(group.quantity, group.quantity != null),
               CostPerUnit = new ValidatedField<float?>(ExcelReportHelper.TryToParseFloat(group.costPerUnit), group.costPerUnit != null),
           })
            .Select(record =>
            {
                record.CommonCost = new ValidatedField<float?>(
                   record.Quantity.Value * record.CostPerUnit.Value,
                   (record.Quantity.Value * record.CostPerUnit.Value) != null);

                return record;
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
                costPerUnit = group.First().CostPerUnit,
                exportDays = group.First().ExportDays
            })
           .Select(group => new ReportRecordData
           {
               ExportDays = new ValidatedField<int?>(group.exportDays, group.exportDays.HasValue),
               Name = new ValidatedField<string?>(group.name, group.name != null),
               Unit = new ValidatedField<string?>(group.unit, group.unit != null),
               Quantity = new ValidatedField<float?>(group.quantity, group.quantity != null),
               CostPerUnit = new ValidatedField<float?>(group.costPerUnit, group.costPerUnit.HasValue),
            })
            .Select(record =>
            {
                record.CommonCost = new ValidatedField<float?>(
                   record.Quantity.Value * record.CostPerUnit.Value,
                   (record.Quantity.Value * record.CostPerUnit.Value) != null);

                return record;
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
                costPerUnit = comp.CostComponent,
                exportDays = comp.ExportDays

            })
            .GroupBy(frameComp => frameComp.name)
            .Select(group => new
            {
                name = group.Key,
                unit = group.First().unit,
                quantity = group.Sum(frameComp => frameComp.quantity),
                costPerUnit = group.First().costPerUnit,
                exportDays = group.First().exportDays
            })
           .Select(group => new ReportRecordData
           {
               ExportDays = new ValidatedField<int?>(group.exportDays, group.exportDays.HasValue),
               Name = new ValidatedField<string?>(group.name, group.name != null),
               Unit = new ValidatedField<string?>(group.unit, group.unit != null),
               Quantity = new ValidatedField<float?>(group.quantity, group.quantity != null),
               CostPerUnit = new ValidatedField<float?>(group.costPerUnit, group.costPerUnit.HasValue),
           })
            .Select(record =>
            {
                record.CommonCost = new ValidatedField<float?>(
                   record.Quantity.Value * record.CostPerUnit.Value,
                   (record.Quantity.Value * record.CostPerUnit.Value) != null);

                return record;
            })
            .ToList();


        //формирование списка кронштейнов
        var sensorsHolders = stands
            .SelectMany(stand => stand.StandAdditionalEquips)
            .SelectMany(equip => equip.AdditionalEquip.Purposes)
            .Where(purpose => purpose.Purpose.Contains("Кронштейн")) //сомнительная хрень, хз что брать за источник информации
            .GroupBy(purpose => purpose.Material)
            .Select(group => new
            {
                name = group.Key,
                unit = group.First().Measure,
                quantity = group.Sum(groupElement => groupElement.Quantity),
                costPerUnit = group.First().CostPerUnit,
                exportDays = group.First().ExportDays
            })
            .Select(group => new ReportRecordData
            {
                ExportDays = new ValidatedField<int?>(group.exportDays, group.exportDays.HasValue),
                Name = new ValidatedField<string?>(group.name, group.name != null),
                Unit = new ValidatedField<string?>(group.unit, group.unit != null),
                Quantity = new ValidatedField<float?>(group.quantity, group.quantity != null),
                CostPerUnit = new ValidatedField<float?>(group.costPerUnit, group.costPerUnit.HasValue),
            })
            .Select(record =>
            {
                record.CommonCost = new ValidatedField<float?>(
                   record.Quantity.Value * record.CostPerUnit.Value,
                   (record.Quantity.Value * record.CostPerUnit.Value) != null);

                return record;
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
                costPerUnit = group.First().CostPerUnit,
                exportDays = group.First().ExportDays
            })
           .Select(group => new ReportRecordData
            {
               ExportDays = new ValidatedField<int?>(group.exportDays, group.exportDays.HasValue),
               Name = new ValidatedField<string?>(group.name, group.name != null),
               Unit = new ValidatedField<string?>(group.unit, group.unit != null),
               Quantity = new ValidatedField<float?>(group.quantity, group.quantity != null),
               CostPerUnit = new ValidatedField<float?>(group.costPerUnit, group.costPerUnit.HasValue),
            })
            .Select(record =>
            {
                record.CommonCost = new ValidatedField<float?>(
                   record.Quantity.Value * record.CostPerUnit.Value,
                   (record.Quantity.Value * record.CostPerUnit.Value) != null);

                return record;
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
                costPerUnit = group.First().CostPerUnit,
                exportDays = group.First().ExportDays
            })
           .Select(group => new ReportRecordData
           {
               ExportDays = new ValidatedField<int?>(group.exportDays, group.exportDays.HasValue),
               Name = new ValidatedField<string?>(group.name, group.name != null),
               Unit = new ValidatedField<string?>(group.unit, group.unit != null),
               Quantity = new ValidatedField<float?>(group.quantity, group.quantity != null),
               CostPerUnit = new ValidatedField<float?>(group.costPerUnit, group.costPerUnit.HasValue),
           })
            .Select(record =>
            {
                record.CommonCost = new ValidatedField<float?>(
                   record.Quantity.Value * record.CostPerUnit.Value,
                   (record.Quantity.Value * record.CostPerUnit.Value) != null);

                return record;
            })
            .Except(sensorsHolders);



        var othersParts = additionalParts
            .Where(part => part.Name.Value != null)
            .Where(part => part.Name.Value.Contains("Шильдик") || part.Name.Value.Contains("Табличка")) //тоже сомнительно
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
    
    //создаем инфу о трудозатратах
    private SummaryReportLaborData GenerateLaborData(IEnumerable<Stand> stands)
    {

        var frameSettings = CalculationSettingsManager.Load<FrameSettings, FrameSettingsData>();
        var electicalSettings = CalculationSettingsManager.Load<ElectricalSettings, ElectricalSettingsData>();
        var humanCostSettings = CalculationSettingsManager.Load<HumanCostSettings, HumanCostSettingsData>();
        var standSettings = CalculationSettingsManager.Load<StandSettings, StandSettingsData>();
        var sandblastSettings = CalculationSettingsManager.Load<SandBlastSettings, SandBlastSettingsData>();




        //трудозатраты на изготовление
        var frameProductionHumanCostSum = stands
            .Select(_ => frameSettings?.TimeForProductionFrame)
            .Aggregate((thisTimeCost, nextTimeCost) => thisTimeCost + nextTimeCost);

        var frameProductionRecord = new ReportRecordData
        {
            ExportDays = new ValidatedField<int?>(null, true),
            Name = new ValidatedField<string?>("Изготовление рам", true),
            Unit = new ValidatedField<string?>("чел/час",true),
            Quantity = new ValidatedField<float?>((float?)frameProductionHumanCostSum, frameProductionHumanCostSum.HasValue),
            CostPerUnit = new ValidatedField<float?>((float?)frameSettings?.FrameProduction, (frameSettings?.FrameProduction != null)),   
            CommonCost = new ValidatedField<float?> (
                (float?)(frameProductionHumanCostSum * frameSettings?.FrameProduction),
                (frameProductionHumanCostSum * frameSettings?.FrameProduction) != null)
            
        };



        //трудозатраты на обвязки
        var allObvHumanCosts = stands
            .SelectMany(stand => stand.ObvyazkiInStand)
            .Select(obv => obv.HumanCost);

        var obvProductionRecord = new ReportRecordData
        {
            ExportDays = new ValidatedField<int?>(null, true),
            Name = new ValidatedField<string?>("Изготовление обвязок", true),
            Unit = new ValidatedField<string?>("чел/час", true),
            Quantity = new ValidatedField<float?>( allObvHumanCosts.Sum(), allObvHumanCosts.All(cost => cost.HasValue)),
            CostPerUnit = new ValidatedField<float?> ((float?)humanCostSettings?.ObvzyakaProduction, (humanCostSettings?.ObvzyakaProduction != null))        
        };

        obvProductionRecord.CommonCost = new ValidatedField<float?>(
            obvProductionRecord.Quantity.Value * obvProductionRecord.CostPerUnit.Value,
            (obvProductionRecord.Quantity.Value * obvProductionRecord.CostPerUnit.Value) != null && obvProductionRecord.Quantity.IsValid);



        //трудозатраты на коллектор
        var collectorProductionHumanCostSum = stands
            .Select(_ => humanCostSettings?.TimeForCollectorBoil)
            .Aggregate((thisTimeCost, nextTimeCost) => thisTimeCost + nextTimeCost);

        var collectorProductionRecord = new ReportRecordData
        {
            ExportDays = new ValidatedField<int?>(null, true),
            Name = new ValidatedField<string?>("Изготовление коллектора", true),
            Unit = new ValidatedField<string?>("чел/час", true),
            Quantity = new ValidatedField<float?>((float?)collectorProductionHumanCostSum, collectorProductionHumanCostSum.HasValue),
            CostPerUnit = new ValidatedField<float?>((float?)humanCostSettings?.CollectorProduction, (humanCostSettings?.CollectorProduction != null)),
            CommonCost = new ValidatedField<float?>(
                (float?)(collectorProductionHumanCostSum * humanCostSettings?.CollectorProduction),
                (collectorProductionHumanCostSum * humanCostSettings?.CollectorProduction) != null)
        };

 



        //трудозатраты на испытания
        var testsHumanCostSum = stands
              .Select(_ => humanCostSettings?.TimeForCheckStand)
              .Aggregate((thisTimeCost, nextTimeCost) => thisTimeCost + nextTimeCost);

        var qualityTestRecord = new ReportRecordData
        {
            ExportDays = new ValidatedField<int?>(null, true),
            Name = new ValidatedField<string?>("Испытание на прочность и герметичность", true),
            Unit = new ValidatedField<string?>("чел/час", true),
            Quantity = new ValidatedField<float?>((float?)testsHumanCostSum, testsHumanCostSum.HasValue),
            CostPerUnit = new ValidatedField<float?>((float?)humanCostSettings?.Tests, (humanCostSettings?.Tests != null)),
            CommonCost = new ValidatedField<float?>(
                (float?)(testsHumanCostSum * humanCostSettings?.Tests),
                (testsHumanCostSum * humanCostSettings?.Tests) != null)
        };





        //трудозатраты на пескоструйные работы
        var sandBlastingHumanCostSum = stands
            .Select(_ => sandblastSettings.TimeSandBlastWork)
            .Aggregate((thisTimeCost, nextTimeCost) => thisTimeCost + nextTimeCost);

        var sandblastingRecord = new ReportRecordData
        {
            ExportDays = new ValidatedField<int?>(null, true),
            Name = new ValidatedField<string?>("Пескоструйные работы", true),
            Unit = new ValidatedField<string?>("чел/час", true),
            Quantity = new ValidatedField<float?>((float?)sandBlastingHumanCostSum, sandBlastingHumanCostSum != null),
            CostPerUnit = new ValidatedField<float?>((float?)sandblastSettings?.SandBlastWork, (sandblastSettings?.SandBlastWork != null)),
            CommonCost = new ValidatedField<float?>(
                (float?)(sandBlastingHumanCostSum * sandblastSettings?.SandBlastWork),
                (sandBlastingHumanCostSum * sandblastSettings?.SandBlastWork) != null)

        };



        //трудозатраты на покраску
        var paintingHumanCostSum = stands
          .Select(_ => (frameSettings?.TimeForPaintFrame + frameSettings?.TimeForPaintObv))
          .Aggregate((thisTimeCost, nextTimeCost) => thisTimeCost + nextTimeCost);


        var paintingRecord = new ReportRecordData
        {
            ExportDays = new ValidatedField<int?>(null, true),
            Name = new ValidatedField<string?>("Покраска", true),
            Unit = new ValidatedField<string?>("чел/час", true),
            Quantity = new ValidatedField<float?>((float?)paintingHumanCostSum, paintingHumanCostSum.HasValue),
            CostPerUnit = new ValidatedField<float?>((float?)frameSettings?.Painting, (frameSettings?.Painting != null)),
            CommonCost = new ValidatedField<float?>(
                (float?)(paintingHumanCostSum * frameSettings?.Painting),
                (paintingHumanCostSum * frameSettings?.Painting) != null)
        };



        //трудозатраты на электромонтаж
        var electricHumanCost = stands
            .Select(_ => electicalSettings?.TimeMontageCable + electicalSettings?.TimeMontageWire)
            .Aggregate((thisTimeCost, nextTimeCost) => thisTimeCost + nextTimeCost);

        var electricRecord = new ReportRecordData
        {
            ExportDays = new ValidatedField<int?>(null, true),
            Name = new ValidatedField<string?>("Электромонтаж", true),
            Unit = new ValidatedField<string?>("чел/час", true),
            Quantity = new ValidatedField<float?>((float?)electricHumanCost, electricHumanCost.HasValue),
            CostPerUnit = new ValidatedField<float?>((float?)electicalSettings?.ElectricalMontage, (electicalSettings?.ElectricalMontage != null)),
            CommonCost = new ValidatedField<float?>(
                (float?)(electricHumanCost * electicalSettings?.ElectricalMontage),
                (electricHumanCost * electicalSettings?.ElectricalMontage) != null)
        };



        //трудозатраты на общую проверку стенда
        var commonHumanCost = stands
            .Select(_ => humanCostSettings?.TimeForCheckStand)
            .Aggregate((thisTimeCost, nextTimeCost) => thisTimeCost + nextTimeCost);


        var commonCheckRecord = new ReportRecordData
        {
            ExportDays = new ValidatedField<int?>(null, true),
            Name = new ValidatedField<string?>("Общая проверка стенда", true),
            Unit = new ValidatedField<string?>("чел/час", true),
            Quantity = new ValidatedField<float?>((float?)commonHumanCost, commonHumanCost.HasValue),
            CostPerUnit = new ValidatedField<float?>((float?)humanCostSettings?.CommonCheckStand, (humanCostSettings?.CommonCheckStand != null)),
            CommonCost = new ValidatedField<float?>(
                (float?)(commonHumanCost * humanCostSettings?.CommonCheckStand),
                (commonHumanCost * humanCostSettings?.CommonCheckStand) != null)
        };


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

    //высчитываем итого по записям
    private ReportRecordData GenerateTotalRecord(IEnumerable<ReportRecordData> records)
    {
        var commonCostsFields = records
            .Select(record => record.CommonCost);

        var exportDaysField = records
            .Select(record => record.ExportDays);
       
        var quantityField = records
            .Select(record => record.Quantity);

        var costField = records
            .Select(record => record.CostPerUnit);



        return new ReportRecordData
        {

            ExportDays = new ValidatedField<int?>(
                exportDaysField.Sum(field => field.Value),
                exportDaysField.All(field => field.IsValid)),

            Quantity = new ValidatedField<float?>(
                quantityField.Sum(field => field.Value),
                quantityField.All(field => field.IsValid)),


            CostPerUnit = new ValidatedField<float?>(
                costField.Sum(field => field.Value),
                costField.All(field => field.IsValid)),


            CommonCost = new ValidatedField<float?>(
                 commonCostsFields.Sum(field => field.Value),
                 commonCostsFields.All(field => field.IsValid)),

            Name = new ValidatedField<string?>(null, true),
            Unit = new ValidatedField<string?>(null, true)
        };

    
    }

    //валидация и вывод в таблицу
    private void PasteRecord(int row, ReportRecordData partRecord, IXLWorksheet ws)
    {

        if (partRecord.ExportDays.Value.HasValue)
        {
            ws.Cell($"A{row}").Value = partRecord.ExportDays.Value.ToString();
        }
        if (!partRecord.ExportDays.IsValid)
        {
            ws.Cell($"A{row}").Value += "\n" + ExcelReportHelper.CommonErrorString;
        }



        if (partRecord.Name.Value != null)
        {
            ws.Cell($"B{row}").Value = partRecord.Name.Value.ToString();
        }
        if (!partRecord.Name.IsValid)
        {
            ws.Cell($"B{row}").Value += "\n" + ExcelReportHelper.CommonErrorString;
        }




        if (partRecord.Unit.Value != null)
        {
            ws.Cell($"C{row}").Value = partRecord.Unit.Value.ToString();
        }
        if (!partRecord.Unit.IsValid)
        {
            ws.Cell($"C{row}").Value += "\n" + ExcelReportHelper.CommonErrorString;
        }



        if (partRecord.Quantity.Value.HasValue)
        {
            ws.Cell($"D{row}").Value = partRecord.Quantity.Value.ToString();
        }
        if (!partRecord.Quantity.IsValid)
        {
            ws.Cell($"D{row}").Value += "\n" + ExcelReportHelper.CommonErrorString;
        }


        if (partRecord.CostPerUnit.Value.HasValue)
        {
            ws.Cell($"E{row}").Value = partRecord.CostPerUnit.Value.ToString();
        }
        if (!partRecord.CostPerUnit.IsValid)
        {
            ws.Cell($"E{row}").Value += "\n" + ExcelReportHelper.CommonErrorString;
        }



        if (partRecord.CommonCost.Value.HasValue)
        {
            ws.Cell($"F{row}").Value = partRecord.CommonCost.Value.ToString();
        }
        if (!partRecord.CommonCost.IsValid)
        {
            ws.Cell($"F{row}").Value += "\n" + ExcelReportHelper.CommonErrorString;
        }

    }



    private List<ReportRecordData> GenerateAllPartsCollection(SummaryReportStandsData partsData)
    {
        //складываем все поля record в общий список
        var allPartsList = new List<ReportRecordData>();
        var partsDataProperties = partsData.GetType().GetProperties();

        foreach (var property in partsDataProperties)
        {
            var propertyValue = property.GetValue(partsData);
            var recordList = propertyValue as List<ReportRecordData>;

            if (recordList != null)
            {
                allPartsList.AddRange(recordList);
            }
        }

        return allPartsList;
    }

    private List<ReportRecordData> GenerateAllLaborsCollection(SummaryReportLaborData partsData)
    {
        //складываем все поля record в общий список
        var allPartsList = new List<ReportRecordData>();
        var partsDataProperties = partsData.GetType().GetProperties();

        foreach (var property in partsDataProperties)
        {
            var propertyValue = property.GetValue(partsData);

            if (propertyValue is ReportRecordData recordList)
            {
                allPartsList.Add(recordList);
            }
        }

        return allPartsList;
    }

    #endregion


    #region Заголовки

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

    

    //Заполняет подтаблицу и возвращает следующую строку
    private int FillPartsSubtableData(int startRow, List<ReportRecordData> items, IXLWorksheet ws)
    {
        var currentRow = startRow;

        foreach (var item in items)
        {
            PasteRecord(currentRow, item, ws);

            ws.Cell($"A{currentRow}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell($"B{currentRow}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            ws.Cell($"C{currentRow}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell($"D{currentRow}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            ws.Cell($"E{currentRow}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            ws.Cell($"F{currentRow}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

            currentRow++;
        }

        return currentRow;
    }

    //создает записи трудозатрат
    private int FillLaborCostSubtable(int startRow, IXLWorksheet ws, SummaryReportLaborData laborCostData)
    {
        var activeRow = startRow;

        var laborRecordProperties = laborCostData.GetType().GetProperties();

        //обрабатываем все стоимости из каждого перечня
        foreach (var property in laborRecordProperties)
        {
            var propertyValue = property.GetValue(laborCostData);
            ReportRecordData laborRecord = (ReportRecordData)propertyValue;


            PasteRecord(activeRow, laborRecord, ws);

            ws.Cell($"A{activeRow}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell($"B{activeRow}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            ws.Cell($"C{activeRow}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Cell($"D{activeRow}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            ws.Cell($"E{activeRow}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            ws.Cell($"F{activeRow}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;



            activeRow++;
        }

        return activeRow;
    }

    //заполняет таблицу для стенда
    private void FillStandTable(IXLWorksheet ws, Stand stand)
    {
        var activeRow = 4;

        var standList = new List<Stand> { stand };

        var generatedPartsData = GeneratePartsData(standList);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Сортамент труб", ws);
        activeRow = FillPartsSubtableData(activeRow, generatedPartsData.PipesList, ws);
        activeRow = CreateGroupTotalRecord(activeRow, generatedPartsData.PipesList, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Арматура", ws);
        activeRow = FillPartsSubtableData(activeRow, generatedPartsData.ArmaturesList, ws);
        activeRow = CreateGroupTotalRecord(activeRow, generatedPartsData.ArmaturesList, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Тройники и КМЧ", ws);
        activeRow = FillPartsSubtableData(activeRow, generatedPartsData.TreeList, ws);
        activeRow = FillPartsSubtableData(activeRow, generatedPartsData.KmchList, ws);

        //общий список, чтобы запихнуть в метод
        var treeAndKmchList = new List<ReportRecordData>();
        treeAndKmchList.AddRange(generatedPartsData.TreeList);
        treeAndKmchList.AddRange(generatedPartsData.KmchList);

        activeRow = CreateGroupTotalRecord(activeRow, treeAndKmchList, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Дренаж", ws);
        activeRow = FillPartsSubtableData(activeRow, generatedPartsData.DrainageParts, ws);
        activeRow = CreateGroupTotalRecord(activeRow, generatedPartsData.DrainageParts, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Рамные комплектующие", ws);
        activeRow = FillPartsSubtableData(activeRow, generatedPartsData.FramesList, ws);
        activeRow = CreateGroupTotalRecord(activeRow, generatedPartsData.FramesList, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Кронштейны", ws);
        activeRow = FillPartsSubtableData(activeRow, generatedPartsData.SensorsHolders, ws);
        activeRow = CreateGroupTotalRecord(activeRow, generatedPartsData.SensorsHolders, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Электрические компоненты", ws);
        activeRow = FillPartsSubtableData(activeRow, generatedPartsData.ElectricalParts, ws);
        activeRow = CreateGroupTotalRecord(activeRow, generatedPartsData.ElectricalParts, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Прочие", ws);
        activeRow = FillPartsSubtableData(activeRow, generatedPartsData.OthersParts, ws);
        activeRow = CreateGroupTotalRecord(activeRow, generatedPartsData.OthersParts, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Расходные материалы", ws);
        activeRow = FillPartsSubtableData(activeRow, generatedPartsData.Supplies, ws);
        activeRow = CreateGroupTotalRecord(activeRow, generatedPartsData.Supplies, ws);

        var allPartsList = GenerateAllPartsCollection(generatedPartsData);
        activeRow = CreatePartsTotalRecord(activeRow, allPartsList, ws);


        var generatedLaborData = GenerateLaborData(standList);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Трудозатраты", ws);
        activeRow = FillLaborCostSubtable(activeRow, ws, generatedLaborData);

        var allLaborsList = GenerateAllLaborsCollection(generatedLaborData);
        activeRow = CreateLaborTotalRecord(activeRow, allLaborsList, ws);


        var allData = new List<ReportRecordData>();

        allData.AddRange(allPartsList);
        allData.AddRange(allLaborsList);

        activeRow = CreatePartsAndLaborTotalRecord(activeRow, allData, ws);
    }

    //заполняет сводную ведомость
    private void FillCommonListTable(IXLWorksheet ws, ProjectInfo project)
    {
        var generatedPartsData = GeneratePartsData(project.Stands);

        var activeRow = 4;

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Сортамент труб", ws);
        activeRow = FillPartsSubtableData(activeRow, generatedPartsData.PipesList, ws);
        activeRow = CreateGroupTotalRecord(activeRow, generatedPartsData.PipesList, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Арматура", ws);
        activeRow = FillPartsSubtableData(activeRow, generatedPartsData.ArmaturesList, ws);
        activeRow = CreateGroupTotalRecord(activeRow, generatedPartsData.ArmaturesList, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Тройники и КМЧ", ws);
        activeRow = FillPartsSubtableData(activeRow, generatedPartsData.TreeList, ws);
        activeRow = FillPartsSubtableData(activeRow, generatedPartsData.KmchList, ws);

        //общий список, чтобы запихнуть в метод
        var treeAndKmchList = new List<ReportRecordData>();
        treeAndKmchList.AddRange(generatedPartsData.TreeList);
        treeAndKmchList.AddRange(generatedPartsData.KmchList);

        activeRow = CreateGroupTotalRecord(activeRow, treeAndKmchList, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Дренаж", ws);
        activeRow = FillPartsSubtableData(activeRow, generatedPartsData.DrainageParts, ws);
        activeRow = CreateGroupTotalRecord(activeRow, generatedPartsData.DrainageParts, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Рамные комплектующие", ws);
        activeRow = FillPartsSubtableData(activeRow, generatedPartsData.FramesList, ws);
        activeRow = CreateGroupTotalRecord(activeRow, generatedPartsData.FramesList, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Кронштейны", ws);
        activeRow = FillPartsSubtableData(activeRow, generatedPartsData.SensorsHolders, ws);
        activeRow = CreateGroupTotalRecord(activeRow, generatedPartsData.SensorsHolders, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Электрические компоненты", ws);
        activeRow = FillPartsSubtableData(activeRow, generatedPartsData.ElectricalParts, ws);
        activeRow = CreateGroupTotalRecord(activeRow, generatedPartsData.ElectricalParts, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Прочие", ws);
        activeRow = FillPartsSubtableData(activeRow, generatedPartsData.OthersParts, ws);
        activeRow = CreateGroupTotalRecord(activeRow, generatedPartsData.OthersParts, ws);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Расходные материалы", ws);
        activeRow = FillPartsSubtableData(activeRow, generatedPartsData.Supplies, ws);
        activeRow = CreateGroupTotalRecord(activeRow, generatedPartsData.Supplies, ws);


        var allPartsList = GenerateAllPartsCollection(generatedPartsData);
        activeRow = CreatePartsTotalRecord(activeRow, allPartsList, ws);

        

        var generatedLaborData = GenerateLaborData(project.Stands);

        activeRow = CreateSubheaderOnWorksheet(activeRow, "Трудозатраты", ws);
        activeRow = FillLaborCostSubtable(activeRow, ws, generatedLaborData);

        var allLaborsList = GenerateAllLaborsCollection(generatedLaborData);
        activeRow = CreateLaborTotalRecord(activeRow, allLaborsList, ws);

        var allData = new List<ReportRecordData>();

        allData.AddRange(allPartsList);
        allData.AddRange(allLaborsList);

        activeRow = CreatePartsAndLaborTotalRecord(activeRow, allData, ws);

    }

    //заполняет лист калькуляции
    private void FillCalculationTable(IXLWorksheet ws, ProjectInfo project)
    {

    }


    #endregion



    #region Итоговые

   
    private int CreateGroupTotalRecord(int row, List<ReportRecordData> recordsList, IXLWorksheet ws)
    {

        var activeRow = row;

        var totalRecord = GenerateTotalRecord(recordsList);
        totalRecord.ExportDays = new ValidatedField<int?>(null, true);
        totalRecord.Name = new ValidatedField<string?>("Итого по категории", true);
        totalRecord.Quantity = new ValidatedField<float?>(null, true);
        totalRecord.CostPerUnit = new ValidatedField<float?>(null, true);

        PasteRecord(activeRow, totalRecord, ws);


        ws.Cell($"B{activeRow}").Style.Font.SetBold();
        ws.Cell($"B{activeRow}").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
        ws.Cell($"F{activeRow}").Style.Font.SetBold();
        ws.Cell($"F{activeRow}").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);


        activeRow++;
        return activeRow;
    }

    private int CreatePartsTotalRecord(int row, List<ReportRecordData> partsRecordsList, IXLWorksheet ws)
    {
        var activeRow = row;

        var totalRecord = GenerateTotalRecord(partsRecordsList);
        totalRecord.ExportDays = new ValidatedField<int?>(null, true);
        totalRecord.Name = new ValidatedField<string?>("Итого по комплектующим", true);
        totalRecord.Quantity = new ValidatedField<float?>(null, true);
        totalRecord.CostPerUnit = new ValidatedField<float?>(null,true);

        PasteRecord(activeRow, totalRecord, ws);


        ws.Cell($"B{activeRow}").Style.Font.SetBold();
        ws.Cell($"B{activeRow}").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
        ws.Cell($"F{activeRow}").Style.Font.SetBold();
        ws.Cell($"F{activeRow}").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);


        activeRow++;
        return activeRow;
    }

    private int CreateLaborTotalRecord(int row, List<ReportRecordData> laborsRecordsList, IXLWorksheet ws)
    {
        var activeRow = row;

        var totalRecord = GenerateTotalRecord(laborsRecordsList);
        totalRecord.ExportDays = new ValidatedField<int?>(null, true);
        totalRecord.Name = new ValidatedField<string?>("Итого по трудозатратам", true);
        totalRecord.CostPerUnit = new ValidatedField<float?>(null, true);

        PasteRecord(activeRow, totalRecord, ws);


        ws.Cell($"B{activeRow}").Style.Font.SetBold();
        ws.Cell($"B{activeRow}").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
        ws.Cell($"D{activeRow}").Style.Font.SetBold();
        ws.Cell($"D{activeRow}").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
        ws.Cell($"F{activeRow}").Style.Font.SetBold();
        ws.Cell($"F{activeRow}").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);


        activeRow++;
        return activeRow;
    }

    private int CreatePartsAndLaborTotalRecord(int row, List<ReportRecordData> recordsList, IXLWorksheet ws)
    {
        var activeRow = row;

        var totalRecord = GenerateTotalRecord(recordsList);
        totalRecord.ExportDays = new ValidatedField<int?>(null, true);
        totalRecord.Name = new ValidatedField<string?>("Итого по комплектующим и трудозатратам", true);
        totalRecord.Quantity = new ValidatedField<float?>(null, true);
        totalRecord.CostPerUnit = new ValidatedField<float?>(null, true);

        PasteRecord(activeRow, totalRecord, ws);


        ws.Cell($"B{activeRow}").Style.Font.SetBold();
        ws.Cell($"B{activeRow}").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Left);
        ws.Cell($"F{activeRow}").Style.Font.SetBold();
        ws.Cell($"F{activeRow}").Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);


        activeRow++;
        return activeRow;
    }

    #endregion
}
