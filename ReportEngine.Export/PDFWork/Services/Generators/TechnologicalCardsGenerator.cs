using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Export.ExcelWork.Enums;
using ReportEngine.Export.ExcelWork.Services.Interfaces;
using ReportEngine.Export.Mapping;
using ReportEngine.Shared.Config.Directory;
using ReportEngine.Shared.Config.IniHeleprs;

namespace ReportEngine.Export.PDFWork.Services.Generators;

public class TechnologicalCardsGenerator : IReportGenerator
{
    private readonly IProjectInfoRepository _projectInfoRepository;

    public TechnologicalCardsGenerator(IProjectInfoRepository projectInfoRepository)
    {
        _projectInfoRepository = projectInfoRepository;
    }

    public ReportType Type => ReportType.TechnologicalCards;

    public async Task GenerateAsync(int projectId)
    {
        var projectInfo = await _projectInfoRepository.GetByIdAsync(projectId);

        var savePath = SettingsManager.GetReportDirectory();

        var fileName = "Технологические карты___" + DateTime.Now.ToString("dd-MM-yy___HH-mm-ss") + ".docx";

        var fullSavePath = Path.Combine(savePath, fileName);

        var templatePath = DirectoryHelper.GetReportsTemplatePath("TechnologicalCards_template", ".docx");

       // var template = Open(templatePath);
        
       // template.BindModel("", TemplateMapper.GetTechnologicalCardsMapping(projectInfo.Stands.First()));
        
       // template.Save(fullSavePath);
    }
}