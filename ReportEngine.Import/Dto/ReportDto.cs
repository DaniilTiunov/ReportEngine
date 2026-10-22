namespace ReportEngine.Import.Dto;

public class ReportDto
{
    public ProjectInfoDto ProjectInfo { get; init; }
    public List<StandDto> Stands { get; init; } = [];
    public List<ComponentDto> AdditionalPurposes { get; init; } = [];
}