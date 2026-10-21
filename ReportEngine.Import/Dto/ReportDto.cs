namespace ReportEngine.Import.Dto;

public class ReportDto
{
    public ProjectInfoDto ProjectInfo { get; init; }
    
    public List<StandDto> Stands { get; init; } = [];
}