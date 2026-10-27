using MapsterMapper;
using ReportEngine.App.Model.StandsModel;
using ReportEngine.App.Services.Interfaces;
using ReportEngine.App.ViewModels;
using ReportEngine.Import.Dto;

namespace ReportEngine.App.Services.Import;

public class ImportService : IImportService
{
    private readonly IMapper _mapper;
    private readonly IProjectService _projectService;
    
    public ImportService(
        IMapper mapper)
    {
        _mapper = mapper;
    }

    public async Task ImportAsync(
        ReportDto report,
        ProjectViewModel projectViewModel)
    {
        await projectViewModel.ResetProjectAsync(true);

        await ImportProjectInfo(report.ProjectInfo, projectViewModel);
        ImportStands(report.Stands, projectViewModel);
    }

    private async Task ImportProjectInfo(
        ProjectInfoDto info,
        ProjectViewModel viewModel)
    {
        _mapper.From(info).AdaptTo(viewModel.CurrentProjectModel);
    }

    private void ImportStands(
        List<StandDto> standDtos,
        ProjectViewModel viewModel)
    {
        var stands =
            _mapper.Map<List<StandModel>>(standDtos);
        
        foreach (var stand in stands)
            viewModel.CurrentProjectModel.Stands.Add(stand);
    }
}