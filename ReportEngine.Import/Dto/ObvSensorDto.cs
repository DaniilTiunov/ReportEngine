namespace ReportEngine.Import.Dto;

public class ObvSensorDto
{
    public string Number { get; set; } = "";

    public string SensorType { get; set; } = "";

    public string KKS { get; set; } = "";

    public string MarkPlus { get; set; } = "";
    public string MarkMinus { get; set; } = "";

    public string Description { get; set; } = "";


    public bool IsEmpty()
    {
        return string.IsNullOrWhiteSpace(Number)
               && string.IsNullOrWhiteSpace(SensorType)
               && string.IsNullOrWhiteSpace(KKS)
               && string.IsNullOrWhiteSpace(MarkPlus)
               && string.IsNullOrWhiteSpace(MarkMinus)
               && string.IsNullOrWhiteSpace(Description);
    }
}