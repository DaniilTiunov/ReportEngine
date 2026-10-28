namespace ReportEngine.Import.Dto;

public class ContainerDto
{
    public string PlaceNumber { get; set; } = "";

    public string ContainerWeight { get; set; } = "";

    public string ContainerName { get; set; } = "";

    public List<StandInContainerDto> Stands { get; set; } = new();
}