using ReportEngine.Domain.Entities;

namespace ReportEngine.App.ModelWrappers;

public static class ObvyzkaModelWrapper
{
    public static ObvyazkaInStand CloneForStand(ObvyazkaInStand source, int newStandId)
    {
        return new ObvyazkaInStand
        {
            // Id не копируем! Он будет сгенерирован БД
            StandId = newStandId,
            ObvyazkaId = source.ObvyazkaId,
            ObvyazkaName = source.ObvyazkaName,
            MaterialLine = source.MaterialLine,
            MaterialLineCount = source.MaterialLineCount,
            MaterialLineMeasure = source.MaterialLineMeasure,
            MaterialLineCostPerUnit = source.MaterialLineCostPerUnit,
            MaterialLineExportDays = source.MaterialLineExportDays,
            TreeSocket = source.TreeSocket,
            TreeSocketMaterialCount = source.TreeSocketMaterialCount,
            TreeSocketMaterialMeasure = source.TreeSocketMaterialMeasure,
            TreeSocketMaterialCostPerUnit = source.TreeSocketMaterialCostPerUnit,
            TreeSocketCount = source.TreeSocketCount,
            TreeSocketExportDays = source.TreeSocketExportDays,
            KMCH = source.KMCH,
            KMCHCount = source.KMCHCount,
            KMCHMeasure = source.KMCHMeasure,
            KMCHCostPerUnit = source.KMCHCostPerUnit,
            KMCHExportDays = source.KMCHExportDays,
            Armature = source.Armature,
            ArmatureCount = source.ArmatureCount,
            ArmatureMeasure = source.ArmatureMeasure,
            ArmatureCostPerUnit = source.ArmatureCostPerUnit,
            ArmatureExportDays = source.ArmatureExportDays,
            NN = source.NN,
            LineLength = source.LineLength,
            ZraCount = source.ZraCount,
            Sensor = source.Sensor,
            SensorType = source.SensorType,
            Clamp = source.Clamp,
            WidthOnFrame = source.WidthOnFrame,
            OtherLineCount = source.OtherLineCount,
            Weight = source.Weight,
            HumanCost = source.HumanCost,
            ImageName = source.ImageName,
            FirstSensorType = source.FirstSensorType,
            FirstSensorKKS = source.FirstSensorKKS,
            FirstSensorMarkPlus = source.FirstSensorMarkPlus,
            FirstSensorMarkMinus = source.FirstSensorMarkMinus,
            FirstSensorDescription = source.FirstSensorDescription,
            SecondSensorType = source.SecondSensorType,
            SecondSensorKKS = source.SecondSensorKKS,
            SecondSensorMarkPlus = source.SecondSensorMarkPlus,
            SecondSensorMarkMinus = source.SecondSensorMarkMinus,
            SecondSensorDescription = source.SecondSensorDescription,
            ThirdSensorType = source.ThirdSensorType,
            ThirdSensorKKS = source.ThirdSensorKKS,
            ThirdSensorMarkPlus = source.ThirdSensorMarkPlus,
            ThirdSensorMarkMinus = source.ThirdSensorMarkMinus,
            ThirdSensorDescription = source.ThirdSensorDescription
        };
    }
}
