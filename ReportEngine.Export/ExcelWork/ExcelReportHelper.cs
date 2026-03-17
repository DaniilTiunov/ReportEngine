using ReportEngine.Domain.Entities;
using ReportEngine.Export.DTO;
using ReportEngine.Shared.Config.IniHelpers;

namespace ReportEngine.Export.ExcelWork;

public static class ExcelReportHelper
{
    public static string CreateReportName(string prefix, string fileExtension)
    {
        return prefix + " " + DateTime.Now.ToString("dd-MM-yy___HH-mm-ss") + "." + fileExtension;
    }

    public static float? TryToParseFloat(string str)
    {
        return float.TryParse(str, out float parseResult) ? parseResult : null;
    }

    public static string CommonErrorString => "Ошибка получения/формирования данных.";


    //костыль для формирования списка датчиков обвязки
    public static List<SensorRecordData> CreateSensorsListFromObvyazka(ObvyazkaInStand obv)
    {
        var resultRecords = new List<SensorRecordData>();

        if (obv.FirstSensorType != null)
            resultRecords.Add(new SensorRecordData(
                obv.FirstSensorKKS ?? "",
                obv.FirstSensorDescription ?? "",
                obv.FirstSensorMarkPlus ?? "",
                obv.FirstSensorMarkMinus ?? ""));

        if (obv.SecondSensorType != null)
            resultRecords.Add(new SensorRecordData(
                obv.SecondSensorKKS ?? "",
                obv.SecondSensorDescription ?? "",
                obv.SecondSensorMarkPlus ?? "",
                obv.SecondSensorMarkMinus ?? ""));

        if (obv.ThirdSensorType != null)
            resultRecords.Add(new SensorRecordData(
                obv.ThirdSensorKKS ?? "",
                obv.ThirdSensorDescription ?? "",
                obv.ThirdSensorMarkPlus ?? "",
                obv.ThirdSensorMarkMinus ?? ""));

        return resultRecords;
    }

    //создаем инфу о комплектующих
    public static PartsStandsData GeneratePartsData(IEnumerable<Stand> stands)
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
                unit = group.FirstOrDefault(group => !string.IsNullOrEmpty(group.units))?.units,
                quantity = group.Sum(pipe => pipe.length),
                costPerUnit = group.FirstOrDefault(group => !string.IsNullOrEmpty(group.price))?.price,
                exportDays = group.FirstOrDefault(group => group.exportDays.HasValue)?.exportDays
            })
            .Where(group => group.quantity != 0.0)
            .Select(group => new EquipmentRecord
            {
                ExportDays = new ValidatedField<int?>(group.exportDays, group.exportDays.HasValue),
                Name = new ValidatedField<string?>(group.name, group.name != null),
                Unit = new ValidatedField<string?>(group.unit, group.unit != null),
                Quantity = new ValidatedField<float?>(group.quantity, group.quantity != null),
                CostPerUnit = new ValidatedField<float?>(TryToParseFloat(group.costPerUnit ?? ""), group.costPerUnit != null),
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
                unit = group.FirstOrDefault(group => !string.IsNullOrEmpty(group.units))?.units,
                quantity = group.Sum(arm => arm.quantity),
                costPerUnit = group.FirstOrDefault(group => !string.IsNullOrEmpty(group.price))?.price,
                exportDays = group.FirstOrDefault(group => group.exportDays.HasValue)?.exportDays
            })
            .Where(group => group.quantity != 0.0)
            .Select(group => new EquipmentRecord
            {
                ExportDays = new ValidatedField<int?>(group.exportDays, group.exportDays.HasValue),
                Name = new ValidatedField<string?>(group.name, group.name != null),
                Unit = new ValidatedField<string?>(group.unit, group.unit != null),
                Quantity = new ValidatedField<float?>(group.quantity, group.quantity != null),
                CostPerUnit = new ValidatedField<float?>(TryToParseFloat(group.costPerUnit ?? ""), group.costPerUnit != null),
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
                unit = group.FirstOrDefault(group => !string.IsNullOrEmpty(group.units))?.units,
                quantity = group.Sum(tree => tree.quantity),
                costPerUnit = group.FirstOrDefault(group => !string.IsNullOrEmpty(group.price))?.price,
                exportDays = group.FirstOrDefault(group => group.exportDays.HasValue)?.exportDays
            })
           .Where(group => group.quantity != 0.0)
           .Select(group => new EquipmentRecord
           {
               ExportDays = new ValidatedField<int?>(group.exportDays, group.exportDays.HasValue),
               Name = new ValidatedField<string?>(group.name, group.name != null),
               Unit = new ValidatedField<string?>(group.unit, group.unit != null),
               Quantity = new ValidatedField<float?>(group.quantity, group.quantity != null),
               CostPerUnit = new ValidatedField<float?>(TryToParseFloat(group.costPerUnit ?? ""), group.costPerUnit != null),
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
                unit = group.FirstOrDefault(group => !string.IsNullOrEmpty(group.units))?.units,
                quantity = group.Sum(groupElement => groupElement.quantity),
                costPerUnit = group.FirstOrDefault(group => !string.IsNullOrEmpty(group.price))?.price,
                exportDays = group.FirstOrDefault(group => group.exportDays.HasValue)?.exportDays
            })
           .Where(group => group.quantity != 0.0)
           .Select(group => new EquipmentRecord
           {
               ExportDays = new ValidatedField<int?>(group.exportDays, group.exportDays.HasValue),
               Name = new ValidatedField<string?>(group.name, group.name != null),
               Unit = new ValidatedField<string?>(group.unit, group.unit != null),
               Quantity = new ValidatedField<float?>(group.quantity, group.quantity != null),
               CostPerUnit = new ValidatedField<float?>(TryToParseFloat(group.costPerUnit ?? ""), group.costPerUnit != null),
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
                unit = group.FirstOrDefault(group => !string.IsNullOrEmpty(group.Measure))?.Measure,
                quantity = group.Sum(groupElement => groupElement.Quantity),
                costPerUnit = group.FirstOrDefault(group => group.CostPerUnit.HasValue)?.CostPerUnit,
                exportDays = group.FirstOrDefault(group => group.ExportDays.HasValue)?.ExportDays
            })
           .Where(group => group.quantity != 0.0)
           .Select(group => new EquipmentRecord
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
                unit = group.FirstOrDefault(group => !string.IsNullOrEmpty(group.unit))?.unit,
                quantity = group.Sum(frameComp => frameComp.quantity),
                costPerUnit = group.FirstOrDefault(group => group.costPerUnit.HasValue)?.costPerUnit,
                exportDays = group.FirstOrDefault(group => group.exportDays.HasValue)?.exportDays
            })
           .Where(group => group.quantity != 0.0)
           .Select(group => new EquipmentRecord
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
            .Where(purpose => purpose.Purpose != null)
            .Where(purpose => purpose.Purpose.Contains("Кронштейн")) //сомнительно, хз что брать за источник информации
            .GroupBy(purpose => purpose.Material)
            .Select(group => new
            {
                name = group.Key,
                unit = group.FirstOrDefault(group => !string.IsNullOrEmpty(group.Measure))?.Measure,
                quantity = group.Sum(groupElement => groupElement.Quantity),
                costPerUnit = group.FirstOrDefault(group => group.CostPerUnit.HasValue)?.CostPerUnit,
                exportDays = group.FirstOrDefault(group => group.ExportDays.HasValue)?.ExportDays
            })
            .Where(group => group.quantity != 0.0)
            .Select(group => new EquipmentRecord
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
                unit = group.FirstOrDefault(group => !string.IsNullOrEmpty(group.Measure))?.Measure,
                quantity = group.Sum(item => item.Quantity),
                costPerUnit = group.FirstOrDefault(group => group.CostPerUnit.HasValue)?.CostPerUnit,
                exportDays = group.FirstOrDefault(group => group.ExportDays.HasValue)?.ExportDays
            })
           .Where(group => group.quantity != 0.0)
           .Select(group => new EquipmentRecord
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
                unit = group.FirstOrDefault(group => !string.IsNullOrEmpty(group.Measure))?.Measure,
                quantity = group.Sum(item => item.Quantity),
                costPerUnit = group.FirstOrDefault(group => group.CostPerUnit.HasValue)?.CostPerUnit,
                exportDays = group.FirstOrDefault(group => group.ExportDays.HasValue)?.ExportDays
            })
           .Where(group => group.quantity != 0.0)
           .Select(group => new EquipmentRecord
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
            .Except(sensorsHolders)
            .ToList();



        var othersParts = new List<EquipmentRecord?>(additionalParts
            .Where(part => part.Name.Value != null)
            .Where(part => part.Name.Value.Contains("Шильдик") || part.Name.Value.Contains("Табличка")))
            .ToList(); // сомнительно, но окэй
   

        

        var supplies =
            additionalParts
            .Except(othersParts)
            .ToList(); 
        

        return new PartsStandsData
        {
            PipesList = pipesList,
            ArmaturesList = armaturesList,
            TreeList = treeList,
            KmchList = kmchList,
            DrainageParts = drainageParts,
            FramesList = framesList,
            SensorsHolders = sensorsHolders,
            ElectricalParts = electricalParts,
            OthersParts = othersParts,
            Supplies = supplies
        };

    }

    //создаем инфу о трудозатратах
    public static LaborStandsData GenerateLaborData(IEnumerable<Stand> stands)
    {
        //грузим сразу все
        var allSettings = CalculationSettingsManager.LoadAllSettings();

        //вытаскиваем нужные
        var frameSettings = allSettings.FrameSettings;
        var electicalSettings = allSettings.ElectricalSettings;
        var humanCostSettings = allSettings.HumanCostSettings;
        var standSettings = allSettings.StandSettings;
        var sandblastSettings = allSettings.SandBlastSettings;



        //трудозатраты на изготовление рам
        //константа из настроек * кол-во рам
        var frameProductionHumanCostSum = stands
            .Select(stand => frameSettings?.TimeForProductionFrame * stand.StandFrames.Count)
            .Aggregate((thisTimeCost, nextTimeCost) => thisTimeCost + nextTimeCost);

        var frameProductionRecord = new EquipmentRecord
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

        var obvProductionRecord = new EquipmentRecord
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
        //=кол-во отверстий в коллекторе * трудозатраты на коллектор + трудозатраты
        //кол-во отверстий в коллекторе = сумма дренажных линий в каждой обвязке стенда + 1





        //отфильтровываем стенды где есть коллектор
        var standsWithCollector = stands
            .Where(stand =>
            {
                var standDrainagesPurposes = stand.StandDrainages.SelectMany(drainage => drainage.Drainage.Purposes);
                return standDrainagesPurposes.Any(drainagePurpose => drainagePurpose.Purpose == "Основная труба" && !string.IsNullOrEmpty(drainagePurpose.Material));
            });

        var standsWithCollectorExists = standsWithCollector.Any();

        //если такие есть - считаем трудозатраты для них
        double? collectorProductionHumanCostSum = null;

        if (standsWithCollectorExists)
        {
            collectorProductionHumanCostSum = standsWithCollector
            .Select(stand => stand.ObvyazkiInStand.Sum(obv => obv.OtherLineCount) + 1)
            .Select(standHolesCount => humanCostSettings?.TimeForOneDrill * standHolesCount)
            .Select(drillHumanCost => drillHumanCost + humanCostSettings?.TimeForCollectorBoil)
            .Aggregate((thisTimeCost, nextTimeCost) => thisTimeCost + nextTimeCost);
        }

        EquipmentRecord? collectorProductionRecord = standsWithCollectorExists ?
            new EquipmentRecord
            {
                ExportDays = new ValidatedField<int?>(null, true),

                Name = new ValidatedField<string?>("Изготовление коллектора", true),

                Unit = new ValidatedField<string?>("чел/час", true),

                Quantity = new ValidatedField<float?>((float?)collectorProductionHumanCostSum,
                                                        collectorProductionHumanCostSum.HasValue),

                CostPerUnit = new ValidatedField<float?>((float?)humanCostSettings?.CollectorProduction,
                                                    (humanCostSettings?.CollectorProduction != null)),

                CommonCost = new ValidatedField<float?>((float?)(collectorProductionHumanCostSum * humanCostSettings?.CollectorProduction),
                                                            (collectorProductionHumanCostSum * humanCostSettings?.CollectorProduction) != null)
            }
            : null;



        //трудозатраты на испытания
        //время проведения всех испытаний * кол-во обвязок
        var testsHumanCostSum = stands
              .Select(stand => humanCostSettings?.TimeForAllChecks * stand.ObvyazkiInStand.Count)
              .Aggregate((thisTimeCost, nextTimeCost) => thisTimeCost + nextTimeCost);


        var qualityTestRecord = new EquipmentRecord
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
        //везде константа из настроек
        var sandBlastingHumanCostSum = stands
            .Select(_ => sandblastSettings.TimeSandBlastWork)
            .Aggregate((thisTimeCost, nextTimeCost) => thisTimeCost + nextTimeCost);

        var sandblastingRecord = new EquipmentRecord
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
        //???
        var paintingHumanCostSum = stands
          .Select(_ => (frameSettings?.TimeForPaintFrame + frameSettings?.TimeForPaintObv))
          .Aggregate((thisTimeCost, nextTimeCost) => thisTimeCost + nextTimeCost);

        var paintingRecord = new EquipmentRecord
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
        //???
        var electricHumanCost = stands
            .Select(_ => electicalSettings?.TimeMontageCable + electicalSettings?.TimeMontageWire)
            .Aggregate((thisTimeCost, nextTimeCost) => thisTimeCost + nextTimeCost);

        var electricRecord = new EquipmentRecord
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
        //везде константа из настроек
        var commonCheckHumanCost = stands
            .Select(_ => humanCostSettings?.TimeForFinalWork)
            .Aggregate((thisTimeCost, nextTimeCost) => thisTimeCost + nextTimeCost);

        var commonCheckRecord = new EquipmentRecord
        {
            ExportDays = new ValidatedField<int?>(null, true),
            Name = new ValidatedField<string?>("Общая проверка стенда", true),
            Unit = new ValidatedField<string?>("чел/час", true),
            Quantity = new ValidatedField<float?>((float?)commonCheckHumanCost, commonCheckHumanCost.HasValue),
            CostPerUnit = new ValidatedField<float?>((float?)humanCostSettings?.CommonCheckStand, (humanCostSettings?.CommonCheckStand != null)),
            CommonCost = new ValidatedField<float?>(
                (float?)(commonCheckHumanCost * humanCostSettings?.CommonCheckStand),
                (commonCheckHumanCost * humanCostSettings?.CommonCheckStand) != null)
        };


        return new LaborStandsData
        {
            FrameProduction = frameProductionRecord,
            ObvProduction = obvProductionRecord,
            CollectorProduction = collectorProductionRecord,
            QualityTests = qualityTestRecord,
            Sandblasting = sandblastingRecord,
            PaintingWorks = paintingRecord,
            ElectricalWorks = electricRecord,
            CommonStandCheck = commonCheckRecord
        };



    }


    //создаем инфу о упаковке
    public static List<EquipmentRecord> GenerateContainersData(IEnumerable<ContainerBatch> containerBatches)
    {
        var containers = containerBatches
            .SelectMany(batch => batch.Containers)
            .GroupBy(container => container.Name)
            .Select(group => new EquipmentRecord
            {
                ExportDays = new ValidatedField<int?>(null, true),
                Name = new ValidatedField<string?>(group.FirstOrDefault().Name, group.FirstOrDefault().Name != null),
                Unit = new ValidatedField<string?>(null, true),
                Quantity = new ValidatedField<float?>(group.Count(), true),
                CostPerUnit = new ValidatedField<float?>(group.FirstOrDefault().ContainerCost, group.FirstOrDefault().ContainerCost.HasValue),
            })
            .Select(record =>
            {
                record.CommonCost = new ValidatedField<float?>(record.Quantity.Value * record.CostPerUnit.Value,
                                                              (record.Quantity.Value * record.CostPerUnit.Value) != null);
                return record;
            }).
            ToList();

        return containers;
    }

    //высчитываем итого по записям
    public static EquipmentRecord GenerateTotalRecord(IEnumerable<EquipmentRecord> records)
    {
        var commonCostsFields = records
            .Select(record => record.CommonCost);

        var exportDaysField = records
            .Select(record => record.ExportDays);

        var quantityField = records
            .Select(record => record.Quantity);

        var costField = records
            .Select(record => record.CostPerUnit);

        return new EquipmentRecord
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
    public static List<EquipmentRecord> GenerateAllPartsCollection(PartsStandsData partsData)
    {
        //складываем все поля record в общий список
        var allPartsList = new List<EquipmentRecord>();
        var partsDataProperties = partsData.GetType().GetProperties();

        foreach (var property in partsDataProperties)
        {
            var propertyValue = property.GetValue(partsData);
            var recordList = propertyValue as List<EquipmentRecord>;

            if (recordList != null)
            {
                allPartsList.AddRange(recordList);
            }
        }

        return allPartsList;
    }

    //превращает данные о трудозатратах в список
    public static List<EquipmentRecord> GenerateAllLaborsCollection(LaborStandsData partsData)
    {
        //складываем все поля record в общий список
        var allPartsList = new List<EquipmentRecord>();
        var partsDataProperties = partsData.GetType().GetProperties();

        foreach (var property in partsDataProperties)
        {
            var propertyValue = property.GetValue(partsData);

            if (propertyValue is EquipmentRecord recordList)
            {
                allPartsList.Add(recordList);
            }
        }

        return allPartsList;
    }
}
