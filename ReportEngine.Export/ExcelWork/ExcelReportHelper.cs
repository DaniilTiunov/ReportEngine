using System.Globalization;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Entities.CalculationParameters.Enums;
using ReportEngine.Domain.Entities.Pipes;
using ReportEngine.Domain.Store;
using ReportEngine.Export.DTO;

namespace ReportEngine.Export.ExcelWork;

public static class ExcelReportHelper
{
    public static string CommonErrorString => "Ошибка получения/формирования данных.";

    public static string CreateReportName(string prefix, string fileExtension)
    {
        return prefix + " " + DateTime.Now.ToString("dd-MM-yy___HH-mm-ss") + "." + fileExtension;
    }

    private static float? TryToParseFloat(string? str)
    {
        return float.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out var parseResult)
            ? parseResult
            : null;
    }


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
                group.FirstOrDefault(group => group.exportDays.HasValue)?.exportDays
            })
            .Where(group => !string.IsNullOrEmpty(group.name))
            .Where(group => group.quantity != 0.0)
            .Select(group => new EquipmentRecord
            {
                ExportDays = new ValidatedField<int?>(group.exportDays, group.exportDays.HasValue),
                Name = new ValidatedField<string?>(group.name, group.name != null),
                Unit = new ValidatedField<string?>(group.unit, group.unit != null),
                Quantity = new ValidatedField<float?>(group.quantity, group.quantity != null),
                CostPerUnit =
                    new ValidatedField<float?>(TryToParseFloat(group.costPerUnit ?? ""), group.costPerUnit != null)
            })
            .Select(record =>
            {
                record.CommonCost = new ValidatedField<float?>(
                    record.Quantity.Value * record.CostPerUnit.Value,
                    record.Quantity.Value * record.CostPerUnit.Value != null);

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
                group.FirstOrDefault(group => group.exportDays.HasValue)?.exportDays
            })
            .Where(group => !string.IsNullOrEmpty(group.name))
            .Where(group => group.quantity != 0.0)
            .Select(group => new EquipmentRecord
            {
                ExportDays = new ValidatedField<int?>(group.exportDays, group.exportDays.HasValue),
                Name = new ValidatedField<string?>(group.name, group.name != null),
                Unit = new ValidatedField<string?>(group.unit, group.unit != null),
                Quantity = new ValidatedField<float?>(group.quantity, group.quantity != null),
                CostPerUnit =
                    new ValidatedField<float?>(TryToParseFloat(group.costPerUnit ?? ""), group.costPerUnit != null)
            })
            .Select(record =>
            {
                record.CommonCost = new ValidatedField<float?>(
                    record.Quantity.Value * record.CostPerUnit.Value,
                    record.Quantity.Value * record.CostPerUnit.Value != null);

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
                group.FirstOrDefault(group => group.exportDays.HasValue)?.exportDays
            })
            .Where(group => !string.IsNullOrEmpty(group.name))
            .Where(group => group.quantity != 0.0)
            .Select(group => new EquipmentRecord
            {
                ExportDays = new ValidatedField<int?>(group.exportDays, group.exportDays.HasValue),
                Name = new ValidatedField<string?>(group.name, group.name != null),
                Unit = new ValidatedField<string?>(group.unit, group.unit != null),
                Quantity = new ValidatedField<float?>(group.quantity, group.quantity != null),
                CostPerUnit =
                    new ValidatedField<float?>(TryToParseFloat(group.costPerUnit ?? ""), group.costPerUnit != null)
            })
            .Select(record =>
            {
                record.CommonCost = new ValidatedField<float?>(
                    record.Quantity.Value * record.CostPerUnit.Value,
                    record.Quantity.Value * record.CostPerUnit.Value != null);

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
                group.FirstOrDefault(group => group.exportDays.HasValue)?.exportDays
            })
            .Where(group => !string.IsNullOrEmpty(group.name))
            .Where(group => group.quantity != 0.0)
            .Select(group => new EquipmentRecord
            {
                ExportDays = new ValidatedField<int?>(group.exportDays, group.exportDays.HasValue),
                Name = new ValidatedField<string?>(group.name, group.name != null),
                Unit = new ValidatedField<string?>(group.unit, group.unit != null),
                Quantity = new ValidatedField<float?>(group.quantity, group.quantity != null),
                CostPerUnit =
                    new ValidatedField<float?>(TryToParseFloat(group.costPerUnit ?? ""), group.costPerUnit != null)
            })
            .Select(record =>
            {
                record.CommonCost = new ValidatedField<float?>(
                    record.Quantity.Value * record.CostPerUnit.Value,
                    record.Quantity.Value * record.CostPerUnit.Value != null);

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
            .Where(group => !string.IsNullOrEmpty(group.name))
            .Where(group => group.quantity != 0.0)
            .Select(group => new EquipmentRecord
            {
                ExportDays = new ValidatedField<int?>(group.exportDays, group.exportDays.HasValue),
                Name = new ValidatedField<string?>(group.name, group.name != null),
                Unit = new ValidatedField<string?>(group.unit, group.unit != null),
                Quantity = new ValidatedField<float?>(group.quantity, group.quantity != null),
                CostPerUnit = new ValidatedField<float?>(group.costPerUnit, group.costPerUnit.HasValue)
            })
            .Select(record =>
            {
                record.CommonCost = new ValidatedField<float?>(
                    record.Quantity.Value * record.CostPerUnit.Value,
                    record.Quantity.Value * record.CostPerUnit.Value != null);

                return record;
            })
            .ToList();

        //Формирование списка рамных комплектующих
        var framesList = stands
            .SelectMany(stand => stand.StandFrames)
            .SelectMany(fr => fr.Frame.Components)
            .Select(comp =>
            {
                var count = 0.0f;

                //костыль с количеством
                if (comp.Count.HasValue && comp.Count.Value != 0)
                    count = comp.Count.Value;
                else if (comp.Length.HasValue && comp.Length.Value != 0) count = comp.Length.Value;

                return new
                {
                    name = comp.ComponentName,
                    unit = comp.Measure,
                    costPerUnit = comp.CostComponent,
                    exportDays = comp.ExportDays,
                    quantity = count
                };
            })
            .GroupBy(frameComp => frameComp.name)
            .Select(group => new
            {
                name = group.Key,
                group.FirstOrDefault(group => !string.IsNullOrEmpty(group.unit))?.unit,
                quantity = group.Sum(frameComp => frameComp.quantity),
                group.FirstOrDefault(group => group.costPerUnit.HasValue)?.costPerUnit,
                group.FirstOrDefault(group => group.exportDays.HasValue)?.exportDays
            })
            .Where(group => !string.IsNullOrEmpty(group.name))
            .Where(group => group.quantity != 0.0)
            .Select(group => new EquipmentRecord
            {
                ExportDays = new ValidatedField<int?>(group.exportDays, group.exportDays.HasValue),
                Name = new ValidatedField<string?>(group.name, group.name != null),
                Unit = new ValidatedField<string?>(group.unit, group.unit != null),
                Quantity = new ValidatedField<float?>(group.quantity, group.quantity != null),
                CostPerUnit = new ValidatedField<float?>(group.costPerUnit, group.costPerUnit.HasValue)
            })
            .Select(record =>
            {
                record.CommonCost = new ValidatedField<float?>(
                    record.Quantity.Value * record.CostPerUnit.Value,
                    record.Quantity.Value * record.CostPerUnit.Value != null);

                return record;
            })
            .ToList();


        //формирование списка кронштейнов
        var sensorsHolders = stands
            .SelectMany(stand => stand.StandAdditionalEquips)
            .SelectMany(equip => equip.AdditionalEquip.Purposes)
            .Where(purpose =>
            {
                //сомнительно, хз что брать за источник информации
                var purposeContainsTemplate = purpose.Purpose?.Contains("Кронштейн") ?? false;
                var materialContainsTemplate = purpose.Material?.Contains("Кронштейн") ?? false;
                return purposeContainsTemplate || materialContainsTemplate;
            })
            .GroupBy(purpose => purpose.Material)
            .Select(group => new
            {
                name = group.Key,
                unit = group.FirstOrDefault(group => !string.IsNullOrEmpty(group.Measure))?.Measure,
                quantity = group.Sum(groupElement => groupElement.Quantity),
                costPerUnit = group.FirstOrDefault(group => group.CostPerUnit.HasValue)?.CostPerUnit,
                exportDays = group.FirstOrDefault(group => group.ExportDays.HasValue)?.ExportDays
            })
            .Where(group => !string.IsNullOrEmpty(group.name))
            .Where(group => group.quantity != 0.0)
            .Select(group => new EquipmentRecord
            {
                ExportDays = new ValidatedField<int?>(group.exportDays, group.exportDays.HasValue),
                Name = new ValidatedField<string?>(group.name, group.name != null),
                Unit = new ValidatedField<string?>(group.unit, group.unit != null),
                Quantity = new ValidatedField<float?>(group.quantity, group.quantity != null),
                CostPerUnit = new ValidatedField<float?>(group.costPerUnit, group.costPerUnit.HasValue)
            })
            .Select(record =>
            {
                record.CommonCost = new ValidatedField<float?>(
                    record.Quantity.Value * record.CostPerUnit.Value,
                    record.Quantity.Value * record.CostPerUnit.Value != null);

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
            .Where(group => !string.IsNullOrEmpty(group.name))
            .Where(group => group.quantity != 0.0)
            .Select(group => new EquipmentRecord
            {
                ExportDays = new ValidatedField<int?>(group.exportDays, group.exportDays.HasValue),
                Name = new ValidatedField<string?>(group.name, group.name != null),
                Unit = new ValidatedField<string?>(group.unit, group.unit != null),
                Quantity = new ValidatedField<float?>(group.quantity, group.quantity != null),
                CostPerUnit = new ValidatedField<float?>(group.costPerUnit, group.costPerUnit.HasValue)
            })
            .Select(record =>
            {
                record.CommonCost = new ValidatedField<float?>(
                    record.Quantity.Value * record.CostPerUnit.Value,
                    record.Quantity.Value * record.CostPerUnit.Value != null);

                return record;
            })
            .ToList();

        //формирование списка дополнительного комплектующих
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
            .Where(group => !string.IsNullOrEmpty(group.name))
            .Where(group => group.quantity != 0.0)
            .Select(group => new EquipmentRecord
            {
                ExportDays = new ValidatedField<int?>(group.exportDays, group.exportDays.HasValue),
                Name = new ValidatedField<string?>(group.name, group.name != null),
                Unit = new ValidatedField<string?>(group.unit, group.unit != null),
                Quantity = new ValidatedField<float?>(group.quantity, group.quantity != null),
                CostPerUnit = new ValidatedField<float?>(group.costPerUnit, group.costPerUnit.HasValue)
            })
            .Select(record =>
            {
                record.CommonCost = new ValidatedField<float?>(
                    record.Quantity.Value * record.CostPerUnit.Value,
                    record.Quantity.Value * record.CostPerUnit.Value != null);

                return record;
            });


        //в прочие материалы кладем только шильдики и таблички из доп комплектующих
        var othersParts = additionalParts
            .Where(record => (record.Name.Value?.Contains("Табличка") ?? false) ||
                             (record.Name.Value?.Contains("Шильдик") ?? false))
            .ToList();


        //расходные материалы - то осталось из доп комплектующих, за исключением прочих материалов и кронштейнов
        var supplies = additionalParts
            .ExceptBy(othersParts.Select(p => p.Name), part => part.Name)
            .ExceptBy(sensorsHolders.Select(h => h.Name), holder => holder.Name);


        //из расходных материалов также сносим дубликаты существующих позиций
        //соединяем все списки комплектующих, по которым нужно проверить, в один
        var allCollectionToCheck = pipesList
            .Union(armaturesList)
            .Union(treeList)
            .Union(kmchList)
            .Union(drainageParts)
            .Union(framesList)
            .Union(sensorsHolders)
            .Union(electricalParts);


        //проверяем, есть ли в расходных материалах комплектующие, которые уже есть в списках выше
        var duplicateRecords = supplies
            .IntersectBy(allCollectionToCheck.Select(part => part.Name), supply => supply.Name);


        //если дубликаты найдены -
        if (duplicateRecords.Any())
        {
            //соотносим позицию в расходных материалах с найденным дубликатом
            var tempRecords = supplies.Join(
                allCollectionToCheck,
                supply => supply.Name,
                part => part.Name,
                (supply, part) => new
                {
                    Supply = supply,
                    duplicatePart = part
                });

            //для каждой существующей позиции обновляем кол-во и общую стоимость
            foreach (var record in tempRecords)
            {
                var prevInfo = record.duplicatePart;

                record.duplicatePart.Quantity = new ValidatedField<float?>(
                    prevInfo.Quantity.Value + record.Supply.Quantity.Value,
                    prevInfo.Quantity.IsValid);

                record.duplicatePart.CommonCost = new ValidatedField<float?>(
                    record.duplicatePart.Quantity.Value * record.duplicatePart.CostPerUnit.Value,
                    prevInfo.CommonCost.IsValid);
            }

            //исключаем из расходных материалов существующие позиции
            supplies = supplies.ExceptBy(duplicateRecords.Select(r => r.Name), supply => supply.Name);
        }

        //формируем окончательный список расходных материалов
        var suppliesList = supplies.ToList();


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
            Supplies = suppliesList
        };
    }

    //создаем инфу о трудозатратах
    public static LaborStandsData GenerateLaborData(IEnumerable<Stand> stands, ParametersStore store,
        ProjectInfo project, IEnumerable<StainlessPipe> stainlessPipes)
    {
        //трудозатраты на изготовление рам


        //трудозатраты рамы,разборной рамы, стойки, шкафы
        var frameProdTimeValue = store[CalculationParameterType.FrameCost, "FrameProdTime"].Value;
        var frameProdTime = TryToParseFloat(frameProdTimeValue);

        var dissassemledFrameProdTimeValue =
            store[CalculationParameterType.FrameCost, "DisassemblableFrameFabTime"].Value;
        var dissassemledFrameProdTime = TryToParseFloat(dissassemledFrameProdTimeValue);

        var standProdTimeValue = store[CalculationParameterType.FrameCost, "StandFabTime"].Value;
        var standProdTime = TryToParseFloat(standProdTimeValue);

        var cabinetProdTimeValue = store[CalculationParameterType.FrameCost, "CabinetPrepTime"].Value;
        var cabinetProdTime = TryToParseFloat(cabinetProdTimeValue);


        //стоимость рамы, разборной рамы стойки, шкафы
        var frameProdCostValue = store[CalculationParameterType.FrameCost, "FrameFabCost"].Value;
        var frameProdCost = TryToParseFloat(frameProdCostValue);

        var dissassemledFrameProdCostValue =
            store[CalculationParameterType.FrameCost, "DisassemblableFrameFabTime"].Value;
        var dissassemledFrameProdCost = TryToParseFloat(dissassemledFrameProdCostValue);

        var standProdCostValue = store[CalculationParameterType.FrameCost, "StandFabCost"].Value;
        var standProdCost = TryToParseFloat(standProdCostValue);

        var cabinetProdCostValue = store[CalculationParameterType.FrameCost, "CabinetWorkCost"].Value;
        var cabinetProdCost = TryToParseFloat(cabinetProdCostValue);


        var framesCostsInfo = stands
            .SelectMany(stand => stand.StandFrames)
            .Select(standFrame => new
            {
                isDissassembledFrame = standFrame.Frame.Disassembled,
                frameType = standFrame.Frame.FrameType
            })
            .Select(frameInfo =>
            {
                float? humanCost = 0;
                float? moneyCost = 0;

                switch (frameInfo.frameType)
                {
                    case "Рама":
                        humanCost = frameInfo.isDissassembledFrame ?? false ? dissassemledFrameProdTime : frameProdTime;
                        moneyCost = frameInfo.isDissassembledFrame ?? false ? dissassemledFrameProdCost : frameProdCost;
                        break;
                    case "Стойка":
                        humanCost = frameInfo.isDissassembledFrame ?? false ? dissassemledFrameProdTime : standProdTime;
                        moneyCost = frameInfo.isDissassembledFrame ?? false ? dissassemledFrameProdCost : standProdCost;
                        break;
                    case "Шкаф":
                        humanCost = cabinetProdTime;
                        moneyCost = cabinetProdCost;
                        break;
                }

                return new
                {
                    frameInfo.isDissassembledFrame,
                    frameInfo.frameType,
                    humanCost,
                    moneyCost
                };
            });

        ;
        var frameProductionHumanCostSum = new
        {
            humanCostSum = framesCostsInfo.Where(info => info.humanCost.HasValue).Sum(info => info.humanCost.Value),
            isValid = framesCostsInfo.All(info => info.humanCost.HasValue)
        };


        var frameProductionMoneyCostSum = new
        {
            moneyCostSum = frameProdCost, //берется стоимость рамы
            isValid = framesCostsInfo.All(info => info.moneyCost.HasValue)
        };

        var frameProductionRecord = new EquipmentRecord
        {
            ExportDays = new ValidatedField<int?>(null, true),
            Name = new ValidatedField<string?>("Изготовление рам", true),
            Unit = new ValidatedField<string?>("чел/час", true),
            Quantity = new ValidatedField<float?>(frameProductionHumanCostSum.humanCostSum,
                frameProductionHumanCostSum.isValid),
            CostPerUnit = new ValidatedField<float?>(frameProductionMoneyCostSum.moneyCostSum,
                frameProductionMoneyCostSum.isValid),
            CommonCost = new ValidatedField<float?>(frameProductionMoneyCostSum.moneyCostSum * frameProductionHumanCostSum.humanCostSum,
                frameProductionHumanCostSum.isValid && frameProductionMoneyCostSum.isValid)
        };


        //трудозатраты на обвязки
        var allObvHumanCosts = stands
            .SelectMany(stand => stand.ObvyazkiInStand)
            .Sum(obv => stainlessPipes.Any(pipe => pipe.Name == obv.MaterialLine)
                ? obv.HumanCost * 1.2f
                : obv.HumanCost);

        var obvProdCostValue = store[CalculationParameterType.HumanCost, "PipeworkFabCost"].Value;
        var obvProdCost = TryToParseFloat(obvProdCostValue);

        var obvProductionRecord = new EquipmentRecord
        {
            ExportDays = new ValidatedField<int?>(null, true),
            Name = new ValidatedField<string?>("Изготовление обвязок", true),
            Unit = new ValidatedField<string?>("чел/час", true),
            Quantity = new ValidatedField<float?>(allObvHumanCosts, allObvHumanCosts.HasValue),
            CostPerUnit = new ValidatedField<float?>(obvProdCost, obvProdCost.HasValue),
            CommonCost = new ValidatedField<float?>(allObvHumanCosts * obvProdCost,
                (allObvHumanCosts * obvProdCost).HasValue)
        };


        //трудозатраты на коллектор

        //отфильтровываем стенды где есть коллектор
        var standsWithCollector = stands
            .Where(stand =>
            {
                var standDrainagesPurposes = stand.StandDrainages.SelectMany(drainage => drainage.Drainage.Purposes);
                return standDrainagesPurposes.Any(drainagePurpose =>
                    drainagePurpose.Purpose == "Основная труба" && !string.IsNullOrEmpty(drainagePurpose.Material));
            });

        var standsWithCollectorExists = standsWithCollector.Any();


        var oneDrillTimeValue = store[CalculationParameterType.HumanCost, "HoleDrillTime"].Value;
        var oneDrillTime = TryToParseFloat(oneDrillTimeValue);

        var collectorWeldTimeValue = store[CalculationParameterType.HumanCost, "ManifoldWeldTime"].Value;
        var collectorWeldTime = TryToParseFloat(collectorWeldTimeValue);

        var collectorProdCostValue = store[CalculationParameterType.HumanCost, "ManifoldFabCost"].Value;
        var collectorProdCost = TryToParseFloat(collectorProdCostValue);

        EquipmentRecord? collectorProductionRecord = null;

        //если такие есть - считаем трудозатраты для них
        if (standsWithCollectorExists)
        {
            float? collectorProdHumanCostSum = null;

            collectorProdHumanCostSum = standsWithCollector
                .Select(stand => stand.ObvyazkiInStand.Sum(obv => obv.OtherLineCount) + 1)
                .Select(standHolesCount => oneDrillTime * standHolesCount)
                .Sum(drillHumanCost => drillHumanCost + collectorWeldTime);


            collectorProductionRecord = new EquipmentRecord
            {
                ExportDays = new ValidatedField<int?>(null, true),

                Name = new ValidatedField<string?>("Изготовление коллектора", true),

                Unit = new ValidatedField<string?>("чел/час", true),

                Quantity = new ValidatedField<float?>(collectorProdHumanCostSum,
                    collectorProdHumanCostSum.HasValue),

                CostPerUnit = new ValidatedField<float?>(collectorProdCost,
                    collectorProdCost.HasValue),

                CommonCost = new ValidatedField<float?>(collectorProdHumanCostSum * collectorProdCost,
                    collectorProdHumanCostSum * collectorProdCost != null)
            };
        }

        //трудозатраты на испытания

        var allChecksTimeValue = store[CalculationParameterType.HumanCost, "AllTestsTime"].Value;
        var allChecksTime = TryToParseFloat(allChecksTimeValue);

        var testsCostValue = store[CalculationParameterType.HumanCost, "TestBenchTestCost"].Value;
        var testsCost = TryToParseFloat(testsCostValue);

        var testsHumanCostSum = stands.Sum(stand => allChecksTime * stand.ObvyazkiInStand.Count);

        var qualityTestRecord = new EquipmentRecord
        {
            ExportDays = new ValidatedField<int?>(null, true),
            Name = new ValidatedField<string?>("Испытание на прочность и герметичность", true),
            Unit = new ValidatedField<string?>("чел/час", true),
            Quantity = new ValidatedField<float?>(testsHumanCostSum, testsHumanCostSum.HasValue),
            CostPerUnit = new ValidatedField<float?>(testsCost, testsCost.HasValue),
            CommonCost = new ValidatedField<float?>(testsHumanCostSum * testsCost,
                testsHumanCostSum * testsCost != null)
        };


        //трудозатраты на пескоструйные работы

        var sandBlastCostValue = store[CalculationParameterType.SandBlastCost, "SandblastingCost"].Value;
        var sandBlastCost = TryToParseFloat(sandBlastCostValue);

        var sandBlastTimeValue = store[CalculationParameterType.SandBlastCost, "SandblastingTime"].Value;
        var sandBlastTime = TryToParseFloat(sandBlastTimeValue);

        var isGalvanised = project.IsGalvanized;

        var sandBlastingHumanCosts = stands
            .SelectMany(stand => stand.StandFrames)
            .Select(standFrame => standFrame.Frame.FrameType)
            .Select(frameType =>
            {
                var notCabinet = !string.IsNullOrEmpty(frameType) && frameType != "Шкаф";
                return notCabinet && !isGalvanised ? sandBlastTime : 0;
            });


        var sandBlastingHumanCostSum = new
        {
            humanCostSum = sandBlastingHumanCosts.Where(cost => cost.HasValue).Sum(cost => cost.Value),
            isValid = sandBlastingHumanCosts.All(cost => cost.HasValue)
        };

        var sandblastingRecord = new EquipmentRecord
        {
            ExportDays = new ValidatedField<int?>(null, true),
            Name = new ValidatedField<string?>("Пескоструйные работы", true),
            Unit = new ValidatedField<string?>("чел/час", true),
            Quantity = new ValidatedField<float?>(sandBlastingHumanCostSum.humanCostSum,
                sandBlastingHumanCostSum.isValid),
            CostPerUnit = new ValidatedField<float?>(sandBlastCost, sandBlastCost.HasValue),
            CommonCost = new ValidatedField<float?>(
                sandBlastingHumanCostSum.humanCostSum * sandBlastCost,
                sandBlastingHumanCostSum.isValid && sandBlastCost.HasValue)
        };


        //трудозатраты на покраску

        var paintObvTimeValue = store[CalculationParameterType.FrameCost, "PipeworkPaintTime"].Value;
        var paintObvTime = TryToParseFloat(paintObvTimeValue);

        var paintFrameTimeValue = store[CalculationParameterType.FrameCost, "FramePaintTime"].Value;
        var paintFrameTime = TryToParseFloat(paintFrameTimeValue);

        var prepareFrameTimeValue = store[CalculationParameterType.FrameCost, "FramePrepTime"].Value;
        var prepareFrameTime = TryToParseFloat(prepareFrameTimeValue);

        var prepareAllEquipmentTimeValue = store[CalculationParameterType.HumanCost, "EquipmentPrepTime"].Value;
        var prepareAllEquipmentTime = TryToParseFloat(prepareAllEquipmentTimeValue);

        var othersOperationsTimeValue = store[CalculationParameterType.HumanCost, "OtherOpsTime"].Value;
        var othersOperationsTime = TryToParseFloat(othersOperationsTimeValue);

        var paintFrameCostValue = store[CalculationParameterType.FrameCost, "TestBenchPaintCost"].Value;
        var paintFrameCost = TryToParseFloat(paintFrameCostValue);

        var framesPaintInfo = stands
            .SelectMany(stand => stand.StandFrames
                .Select(frame => new
                {
                    Stand = stand, frame.Frame.FrameType
                })
            )
            .Select(frameInfo =>
            {
                var notCabinet = !string.IsNullOrEmpty(frameInfo.FrameType) && frameInfo.FrameType != "Шкаф";
                var framePaintTime = notCabinet && !isGalvanised ? paintFrameTime : 0;
                var needToPaint = (notCabinet && !isGalvanised) || frameInfo.Stand.ObvyazkiInStand.Count > 0;

                return new
                {
                    NeedToPaint = needToPaint,
                    StandHasObv = frameInfo.Stand.ObvyazkiInStand.Count > 0,
                    FrameHumanCost = framePaintTime + prepareFrameTime
                };
            });

        var obvPaintingHumanCost = stands.Sum(stand => stand.ObvyazkiInStand.Count * paintObvTime);

        //возможно,надо считать для каждого стенда
        var equipmentPrepareHumanCost = framesPaintInfo
            .Any(stand => stand.NeedToPaint)
            ? prepareAllEquipmentTime
            : 0;

        var paintingHumanCostSum = framesPaintInfo.Sum(frameInfo => frameInfo.FrameHumanCost) + obvPaintingHumanCost +
                                   equipmentPrepareHumanCost;

        var paintingRecord = new EquipmentRecord
        {
            ExportDays = new ValidatedField<int?>(null, true),
            Name = new ValidatedField<string?>("Покраска", true),
            Unit = new ValidatedField<string?>("чел/час", true),
            Quantity = new ValidatedField<float?>(paintingHumanCostSum, paintingHumanCostSum.HasValue),
            CostPerUnit = new ValidatedField<float?>(paintFrameCost, paintFrameCost.HasValue),
            CommonCost = new ValidatedField<float?>(paintingHumanCostSum * paintFrameCost,
                paintingHumanCostSum * paintFrameCost != null)
        };


        //трудозатраты на электромонтаж

        var oneInputMontageTimeValue = store[CalculationParameterType.HumanCost, "InputInstallTime"].Value;
        var oneInputMontageTime = TryToParseFloat(oneInputMontageTimeValue);

        var oneHoleBusDrillTimeValue = store[CalculationParameterType.HumanCost, "BusbarHoleDrillTime"].Value;
        var oneHoleBusDrillTime = TryToParseFloat(oneHoleBusDrillTimeValue);

        var oneWireMontageTimeValue = store[CalculationParameterType.ElectricCost, "WireInstallTime"].Value;
        var oneWireMontageTime = TryToParseFloat(oneWireMontageTimeValue);

        var oneCableMontageTimeValue = store[CalculationParameterType.ElectricCost, "CableInstallTime"].Value;
        var oneCableMontageTime = TryToParseFloat(oneCableMontageTimeValue);

        var electricMontageCostValue = store[CalculationParameterType.ElectricCost, "ElectricalInstallCost"].Value;
        var electricMontageCost = TryToParseFloat(electricMontageCostValue);

        var electricHumanCost = stands
            .Sum(stand =>
            {
                var cableInputsQuantity = stand.StandElectricalComponent
                    .SelectMany(equip => equip.ElectricalComponent.Purposes)
                    .Where(purpose => purpose.Purpose == "Кабельные вводы")
                    .Sum(record => record.Quantity);

                //затраты на кабельные ввода
                var cableInputsTimeCost = cableInputsQuantity * oneInputMontageTime;

                //затраты на приборы
                bool IsElectricSensor(string? typeOfSensor)
                {
                    return !string.IsNullOrEmpty(typeOfSensor) && typeOfSensor != "Манометр";
                }

                int SensorCounter(ObvyazkaInStand obv)
                {
                    var sensorsQuantity = 0;

                    if (IsElectricSensor(obv.FirstSensorType))
                        sensorsQuantity++;

                    if (IsElectricSensor(obv.SecondSensorType))
                        sensorsQuantity++;

                    if (IsElectricSensor(obv.ThirdSensorType))
                        sensorsQuantity++;

                    return sensorsQuantity;
                }

                var electricSensorsQuantity = stand.ObvyazkiInStand.Sum(SensorCounter);

                var sensorsTimeCost = (electricSensorsQuantity + 2) * oneWireMontageTime +
                                      electricSensorsQuantity * oneCableMontageTime +
                                      (electricSensorsQuantity + 4) * oneHoleBusDrillTime;

                //результат на 1 стенд
                return cableInputsTimeCost +
                       sensorsTimeCost +
                       othersOperationsTime;
            });

        var electricRecord = new EquipmentRecord
        {
            ExportDays = new ValidatedField<int?>(null, true),
            Name = new ValidatedField<string?>("Электромонтаж", true),
            Unit = new ValidatedField<string?>("чел/час", true),
            Quantity = new ValidatedField<float?>(electricHumanCost, electricHumanCost.HasValue),
            CostPerUnit = new ValidatedField<float?>(electricMontageCost, electricMontageCost.HasValue),
            CommonCost = new ValidatedField<float?>(electricHumanCost * electricMontageCost,
                electricHumanCost * electricMontageCost != null)
        };

        //трудозатраты на общую проверку стенда

        var commonCheckCostValue = store[CalculationParameterType.HumanCost, "TestBenchInspectCost"].Value;
        var commonCheckCost = TryToParseFloat(commonCheckCostValue);

        var finalWorksTimeValue = store[CalculationParameterType.HumanCost, "FinalWorkTime"].Value;
        var finalWorksTime = TryToParseFloat(finalWorksTimeValue);

        var commonCheckHumanCost = stands.Count() * finalWorksTime;

        var commonCheckRecord = new EquipmentRecord
        {
            ExportDays = new ValidatedField<int?>(null, true),
            Name = new ValidatedField<string?>("Общая проверка стенда", true),
            Unit = new ValidatedField<string?>("чел/час", true),
            Quantity = new ValidatedField<float?>(commonCheckHumanCost, commonCheckHumanCost.HasValue),
            CostPerUnit = new ValidatedField<float?>(commonCheckCost,
                commonCheckCost.HasValue),
            CommonCost = new ValidatedField<float?>(
                commonCheckHumanCost * commonCheckCost,
                commonCheckHumanCost * commonCheckCost != null)
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
                CostPerUnit = new ValidatedField<float?>(group.FirstOrDefault().ContainerCost,
                    group.FirstOrDefault().ContainerCost.HasValue)
            })
            .Select(record =>
            {
                record.CommonCost = new ValidatedField<float?>(record.Quantity.Value * record.CostPerUnit.Value,
                    record.Quantity.Value * record.CostPerUnit.Value != null);
                return record;
            }).ToList();

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

            if (recordList != null) allPartsList.AddRange(recordList);
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

            if (propertyValue is EquipmentRecord recordList) allPartsList.Add(recordList);
        }

        return allPartsList;
    }
}
