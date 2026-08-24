using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Entities.CalculationParameters.Enums;
using ReportEngine.Domain.Store;
using ReportEngine.Export.DTO;
using ReportEngine.Export.DTO.JsonObjects;
using ReportEngine.Shared.Helpers;

namespace ReportEngine.Export.ExcelWork;

public static class JsonCreator
{
    //создание JSON объекта проекта
    public static async Task<ProjectJsonObject> CreateProjectJson(ProjectInfo project, ParametersStore parametersStore,
        List<Stand>? selectedStands = null)
    {
        var sourceData = selectedStands ?? project.Stands;

        return new ProjectJsonObject
        {
            SeniorEngineer = parametersStore[CalculationParameterType.StandCost, "LeadEngineer"].Value,
            ResponsibleForAccept = parametersStore[CalculationParameterType.StandCost, "AcceptanceSupervisor"].Value,
            SecondLevelSpecialist = parametersStore[CalculationParameterType.StandCost, "SpecialistL2"].Value,
            OSiL = parametersStore[CalculationParameterType.StandCost, "OsilRep"].Value,

            Number = project.Number,
            Id = project.Id,
            Description = project.Description,
            CreationDate = project.CreationDate,
            Company = project.Company,
            Object = project.Object,
            StandCount = project.StandCount,
            Cost = project.Cost,
            Status = project.Status,
            StartDate = project.StartDate,
            OutOfProduction = project.OutOfProduction,
            EndDate = project.EndDate,
            OrderCustomer = project.OrderCustomer,
            RequestProduction = project.RequestProduction,
            MarkPlus = project.MarkPlus,
            MarkMinus = project.MarkMinus,
            IsGalvanized = project.IsGalvanized,
            HumanCost = project.HumanCost,
            Manager = project.Manager,
            Stands = sourceData
                .OrderBy(stand => stand.Number)
                .Select(CreateStandJson)
                .ToList()
        };
    }

    // создание JSON объекта стенда
    private static StandJsonObject CreateStandJson(Stand stand)
    {
        var framesInfos = stand.StandFrames
            .Select(frame => new
            {
                frame.Frame.Width,
                DocName = frame.Frame.Designe,
                frame.Frame.Height,
                frame.Frame.Depth
            })
            .GroupBy(frame => frame.DocName)
            .Select(group => new FrameRecordJsonObject
            {
                Width = group.FirstOrDefault().Width,
                Height = group.FirstOrDefault().Height,
                Depth = group.FirstOrDefault().Depth,
                DocName = group.FirstOrDefault().DocName,
                Quantity = group.Count()
            });

        var parts = ExcelReportHelper.GeneratePartsData(new List<Stand> { stand });

        var framesParts = parts.FramesList.Select(record => RecordToJson(record));

        var drainageParts = parts.DrainageParts.Select(record => RecordToJson(record));

        var electricalParts = parts.ElectricalParts.Select(record => RecordToJson(record));

        var mountPartsRecords = new List<EquipmentRecord>();

        mountPartsRecords.AddRange(parts.PipesList);
        mountPartsRecords.AddRange(parts.ArmaturesList);
        mountPartsRecords.AddRange(parts.TreeList);
        mountPartsRecords.AddRange(parts.KmchList);
        mountPartsRecords.AddRange(parts.SensorsHolders);
        mountPartsRecords.AddRange(parts.OthersParts);
        mountPartsRecords.AddRange(parts.Supplies);


        var mountParts = mountPartsRecords.Select(record => RecordToJson(record));

        var impulseLines = stand.ObvyazkiInStand
            .SelectMany(obv => ExcelReportHelper.CreateSensorsListFromObvyazka(obv))
            .Select(record => SensorToJson(record, stand));

        return new StandJsonObject
        {
            Number = stand.Number,
            KKSCode = stand.KKSCode ?? "",
            Designation = stand.Design ?? "",
            Devices = stand.Devices,
            BraceType = stand.BraceType,
            Width = stand.Width,
            SerialNumber = stand.SerialNumber,
            Weight = stand.Weight,
            StandSummCost = stand.StandSummCost,
            ObvyazkaType = stand.ObvyazkaType,
            NN = stand.NN,
            MaterialLine = stand.MaterialLine,
            Armature = stand.Armature,
            TreeSocket = stand.TreeSocket,
            KMCH = stand.KMCH,
            Description = stand.DesigneStand ?? "",
            Comments = stand.Comments,
            ContainerStandId = stand.ContainerStandId,
            ImageData = stand.ImageData,
            ImageType = stand.ImageType,
            Frames = framesInfos.ToList(),
            FrameParts = framesParts.ToList(),
            DrainageParts = drainageParts.ToList(),
            ElectricParts = electricalParts.ToList(),
            MountParts = mountParts.ToList(),
            ImpulseLines = impulseLines.ToList()
        };
    }

    //конвертация записи детали в JSON объект
    public static PartRecordJsonObject RecordToJson(EquipmentRecord record)
    {
        return new PartRecordJsonObject
        {
            Name = record?.Name.Value,
            Unit = record?.Unit.Value,
            Quantity = record?.Quantity.Value.Round(1)
        };
    }

    //конвертация записи датчика в JSON объект
    public static ImpulseLineRecordJsonObject SensorToJson(SensorRecordData record, Stand stand)
    {
        //находим название коробки в стенде
        var boxName = stand.StandElectricalComponent
            .SelectMany(sec => sec.ElectricalComponent.Purposes)
            .First(purpose => !string.IsNullOrEmpty(purpose.Purpose) && purpose.Purpose.StartsWith("Клеммная коробка"))
            .Material;

        var wiresInfo = new List<WireRecord>
        {
            new("+", $"{record.SensorMarkPlus}", boxName ?? "", "1"),
            new("-", $"{record.SensorMarkMinus}", boxName ?? "", "2"),
            new("Экран", "", boxName ?? "", "3")
        };

        return new ImpulseLineRecordJsonObject
        {
            Name = ExcelReportHelper.RemoveControlSymbols(record.SensorDescription),
            CodeKKS = ExcelReportHelper.RemoveControlSymbols(record.SensorKKS),
            Wires = wiresInfo,
            Annotation = ExcelReportHelper.RemoveControlSymbols(stand.DesigneStand)
        };
    }
}
