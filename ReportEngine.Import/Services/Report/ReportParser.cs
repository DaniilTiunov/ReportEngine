using ReportEngine.Import.Abstractions;
using ReportEngine.Import.Dto;

namespace ReportEngine.Import.Services.Report;

public class ReportParser : IReportParser
{
    private readonly IFileService _fileService;
    private readonly IProjectInfoParser _projectInfoParser;
    private readonly IStandParser _standParser;
        
    public ReportParser(
        IFileService fileService,
        IProjectInfoParser projectInfoParser,
        IStandParser standParser)
    {
        _fileService = fileService;
        _projectInfoParser = projectInfoParser;
        _standParser = standParser;
    }    
    
    public ReportDto Parse(string fileName)
    {
        var workbook = _fileService.OpenFileAsXlw(fileName);
        
        var worksheets = workbook.Worksheets;

        return new ReportDto()
        {
            ProjectInfo = _projectInfoParser.Parse(worksheets.First()),
            Stands = _standParser.Parse(worksheets)
        };
    }
}