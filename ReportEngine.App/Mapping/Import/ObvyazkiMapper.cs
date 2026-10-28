using System.Globalization;
using Mapster;
using ReportEngine.Domain.Entities;
using ReportEngine.Import.Dto;

namespace ReportEngine.App.Mapping.Import;

public class ObvyazkiMapper : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<ObvyazkaDto, ObvyazkaInStand>()
            .MapWith(src => Convert(src));
    }
    
    private static ObvyazkaInStand Convert(ObvyazkaDto src)
    {
        var result = new ObvyazkaInStand
        {
            ObvyazkaId = src.ObvyazkaId,
            NN = ParseInt(src.Number),
            ObvyazkaName = src.ObvType,

            MaterialLine = src.Line.Material,
            MaterialLineCount = ParseFloat(src.Line.Quantity),
            MaterialLineMeasure = src.Line.Measure,
            MaterialLineCostPerUnit = src.Line.CostPerUnit,
            MaterialLineExportDays = ParseInt(src.Line.ExportDays),

            Armature = src.Armature.Material,
            ArmatureCount = ParseFloat(src.Armature.Quantity),
            ArmatureMeasure = src.Armature.Measure,
            ArmatureCostPerUnit = src.Armature.CostPerUnit,
            ArmatureExportDays = ParseInt(src.Armature.ExportDays),

            TreeSocket = src.TreeSocket.Material,
            TreeSocketMaterialCount = ParseFloat(src.TreeSocket.Quantity),
            TreeSocketMaterialMeasure = src.TreeSocket.Measure,
            TreeSocketMaterialCostPerUnit = src.TreeSocket.CostPerUnit,
            TreeSocketExportDays = ParseInt(src.TreeSocket.ExportDays),

            KMCH = src.KMCH.Material,
            KMCHCount = ParseFloat(src.KMCH.Quantity),
            KMCHMeasure = src.KMCH.Measure,
            KMCHCostPerUnit = src.KMCH.CostPerUnit,
            KMCHExportDays = ParseInt(src.KMCH.ExportDays),
            
            ImageName = $"{src.ObvyazkaId}",

            AdditionalComponents = src.ObvAdditionalComponents
                .Select(c => new ObvyazkaAdditionalEquipPurpose
                {
                    Purpose = c.Purpose,
                    Material = c.Material,
                    Quantity = ParseFloat(c.Quantity),
                    Measure = c.Measure,
                    CostPerUnit = ParseFloat(c.CostPerUnit),
                    Weight = ParseFloat(c.Weight),
                    ExportDays = ParseInt(c.ExportDays)
                })
                .ToList()
        };

        var sensors = new Dictionary<int, ObvSensorDto>();

        foreach (var sensor in src.Sensors)
        {
            var number = ParseInt(sensor.Number);

            if (number is not (1 or 2 or 3))
                throw new FormatException(
                    $"Недопустимый номер датчика: '{sensor.Number}'.");

            if (!sensors.TryAdd(number.Value, sensor))
                throw new FormatException(
                    $"Повторяется номер датчика: {number}.");
        }

        sensors.TryGetValue(1, out var first);
        sensors.TryGetValue(2, out var second);
        sensors.TryGetValue(3, out var third);

        result.FirstSensorType = first?.SensorType;
        result.FirstSensorKKS = first?.KKS;
        result.FirstSensorMarkPlus = first?.MarkPlus;
        result.FirstSensorMarkMinus = first?.MarkMinus;
        result.FirstSensorDescription = first?.Description;

        result.SecondSensorType = second?.SensorType;
        result.SecondSensorKKS = second?.KKS;
        result.SecondSensorMarkPlus = second?.MarkPlus;
        result.SecondSensorMarkMinus = second?.MarkMinus;
        result.SecondSensorDescription = second?.Description;

        result.ThirdSensorType = third?.SensorType;
        result.ThirdSensorKKS = third?.KKS;
        result.ThirdSensorMarkPlus = third?.MarkPlus;
        result.ThirdSensorMarkMinus = third?.MarkMinus;
        result.ThirdSensorDescription = third?.Description;

        return result;
    }

    private static float? ParseFloat(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var normalized = value.Trim()
            .Replace(" ", "")
            .Replace("\u00A0", "")
            .Replace("\u202F", "")
            .Replace(',', '.');

        return float.Parse(
            normalized,
            NumberStyles.Float,
            CultureInfo.InvariantCulture);
    }

    private static int? ParseInt(string? value)
    {
        return string.IsNullOrWhiteSpace(value)
            ? null
            : int.Parse(
                value.Trim(),
                NumberStyles.Integer,
                CultureInfo.InvariantCulture);
    }
}
