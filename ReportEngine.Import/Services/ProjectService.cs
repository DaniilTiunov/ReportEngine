using ReportEngine.Import.Models;

namespace ReportEngine.Import.Services;

public class ProjectService
{
    private readonly JsonFileService _jsonFileService;
    private ProjectFile _projectFile;
    
    public ProjectService(JsonFileService jsonFileService)
    {
        _jsonFileService = jsonFileService;

        _projectFile = _jsonFileService.GetProjectFileDataAsync()
                                    .GetAwaiter()
                                    .GetResult();
    }

    public ProjectInfoJsonModel GetProjectInfo()
    {
        return new ProjectInfoJsonModel
        {
            ProjectId = _projectFile.Project.ProjectId,
            DesignationKD = _projectFile.Project.DesignationKD,
            Client = _projectFile.Project.Client,
            Object = _projectFile.Project.Object,
            Status = _projectFile.Project.Status,
            RequestDate = _projectFile.Project.RequestDate,
            ShipmentDate = _projectFile.Project.ShipmentDate,
            StandsCount = _projectFile.Project.StandsCount,
        };
    }

    public IEnumerable<StandJsonModel> GetStands()
    {
        return _projectFile.Stands;
    }

    public IEnumerable<MaterialJsonModel> GetMaterials()
    {
        var materials = 
            _projectFile.Stands.SelectMany(material => material.Materials);
        
        return materials;
    }
}