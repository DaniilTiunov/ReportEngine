using ReportEngine.Import.Abstractions;
using ReportEngine.Import.Dto;

namespace ReportEngine.Import.Services.Report;

public class ReportParser : IReportParser
{
    private readonly IFileService _fileService;
    private readonly IProjectInfoParser _projectInfoParser;
        
    public ReportParser(
        IFileService fileService,
        IProjectInfoParser projectInfoParser)
    {
        _fileService = fileService;
        _projectInfoParser = projectInfoParser;
    }    
    
    public ReportDto Parse(string fileName)
    {
        var workbook = _fileService.OpenFileAsXlw(fileName);
        
        var worksheet = workbook.Worksheets.First();

        return new ReportDto()
        {
            ProjectInfo = _projectInfoParser.Parse(worksheet),
        };
    }
}