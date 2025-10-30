using ReportEngine.Domain.Entities;
using ReportEngine.Export.ExcelWork.Services.Generators.DTO;
using ReportEngine.Shared.Config.IniHelpers;
using ReportEngine.Shared.Config.IniHelpers.CalculationSettings;
using ReportEngine.Shared.Config.IniHelpers.CalculationSettingsData;

namespace ReportEngine.Export.Mapping;

public static class ExcelReportHelper
{

    public static string CreateReportName(string prefix, string fileExtension)
    {
        return prefix + "___" + DateTime.Now.ToString("dd-MM-yy___HH-mm-ss") + "." + fileExtension;
    }

    public static float? TryToParseFloat(string str)
    {
        return float.TryParse(str, out float parseResult) ? parseResult : null;
    }


    public static string CommonErrorString => "Ошибка получения/формирования данных.";

    //создаем инфу о комплектующих
    public static SummaryReportStandsData GeneratePartsData(IEnumerable<Stand> stands)
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
    public static SummaryReportLaborData GenerateLaborData(IEnumerable<Stand> stands)
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
            Unit = new ValidatedField<string?>("чел/час", true),
            Quantity = new ValidatedField<float?>((float?)frameProductionHumanCostSum, frameProductionHumanCostSum.HasValue),
            CostPerUnit = new ValidatedField<float?>((float?)frameSettings?.FrameProduction, (frameSettings?.FrameProduction != null)),
            CommonCost = new ValidatedField<float?>(
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
            Quantity = new ValidatedField<float?>(allObvHumanCosts.Sum(), allObvHumanCosts.All(cost => cost.HasValue)),
            CostPerUnit = new ValidatedField<float?>((float?)humanCostSettings?.ObvzyakaProduction, (humanCostSettings?.ObvzyakaProduction != null))
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
    public static ReportRecordData GenerateTotalRecord(IEnumerable<ReportRecordData> records)
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

    //превращает данные о комплектующих в список
    public static List<ReportRecordData> GenerateAllPartsCollection(SummaryReportStandsData partsData)
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
    //превращает данные о трудозатратах в список
    public static List<ReportRecordData> GenerateAllLaborsCollection(SummaryReportLaborData partsData)
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

}