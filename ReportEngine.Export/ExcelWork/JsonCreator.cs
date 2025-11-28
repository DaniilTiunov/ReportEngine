using ReportEngine.Domain.Entities;
using ReportEngine.Export.DTO;
using ReportEngine.Export.DTO.JsonObjects;
using ReportEngine.Shared.Config.IniHelpers;
using ReportEngine.Shared.Config.IniHelpers.CalculationSettings;
using ReportEngine.Shared.Config.IniHelpers.CalculationSettingsData;



namespace ReportEngine.Export.ExcelWork
{
    public static class JsonCreator
    {
        //создание JSON объекта проекта
        public static ProjectJsonObject CreateProjectJson(ProjectInfo project)
        {

            var standSettings = CalculationSettingsManager.Load<StandSettings, StandSettingsData>();

            return new ProjectJsonObject
            {
                SeniorEngineer = standSettings.SeniorEngineer,
                ResponsibleForAccept = standSettings.ResponsibleForAccept,
                SecondLevelSpecialist = standSettings.SecondLevelSpecialist,
                OSiL = standSettings.OSiL,

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
                Stands = project.Stands.Select(stand => CreateStandJson(stand)).ToList()
            };
        }

        // создание JSON объекта стенда
        private static StandJsonObject CreateStandJson(Stand stand)
        {
            var framesInfos = stand.StandFrames
                .Select(frame => new
                {
                    Width = frame.Frame.Width,
                    DocName = frame.Frame.Designe,
                    Height = frame.Frame.Height,
                    Depth = frame.Frame.Depth
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

            var mountParts = mountPartsRecords.Select(record => RecordToJson(record));

            var impulseLines = stand.ObvyazkiInStand
                .SelectMany(obv => ExcelReportHelper.CreateSensorsListFromObvyazka(obv))
                .Select(record => SensorToJson(record));

            return new StandJsonObject
            {
                Number = stand.Number,
                KKSCode = stand.KKSCode,
                Designation = stand.Design,
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
                Description = stand.DesigneStand,
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
                Name = record.Name.Value,
                Unit = record.Unit.Value,
                Quantity = record.Quantity.Value
            };
        }

        //конвертация записи датчика в JSON объект
        public static ImpulseLineRecordJsonObject SensorToJson(SensorRecordData record)
        {
            var wiresInfo = new List<WireRecord>()
            { 
                new WireRecord("+",$"{record.SensorKKS}+","Коробка КС-1.6","1"),
                new WireRecord("-",$"{record.SensorKKS}-","Коробка КС-1.6","2"),
                new WireRecord("Экран","","Коробка КС-1.6","3")
            };

            return new ImpulseLineRecordJsonObject
            {
                Name = record.SensorDescription,
                CodeKKS = record.SensorKKS,
                Wires = wiresInfo,
                Annotation = ""
            };
        }
    }
}
