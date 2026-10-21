using Mapster;
using MapsterMapper;
using ReportEngine.App.Model.StandsModel;
using ReportEngine.App.ViewModels;
using ReportEngine.Import.Dto;

namespace ReportEngine.App.Services.Import;

public class ImportService : IImportService
{
    private readonly IMapper _mapper;

    public ImportService(IMapper mapper)
    {
        _mapper = mapper;
    }
    
    public void Import(
        ReportDto report,
        ProjectViewModel projectViewModel)
    {
        projectViewModel.ResetProject();
        
        ImportProjectInfo(report.ProjectInfo,  projectViewModel);
        ImportStands(report.Stands, projectViewModel);
    }
    
    private void ImportProjectInfo(
        ProjectInfoDto info,
        ProjectViewModel viewModel)
    {
        _mapper.From(info).AdaptTo(viewModel.CurrentProjectModel);
    }

    private void ImportStands(
        List<StandDto> standDtos,
        ProjectViewModel viewModel)
    {
        List<StandModel> stands =
            _mapper.Map<List<StandModel>>(standDtos);

        foreach (var stand in stands)
        {
            viewModel.CurrentProjectModel.Stands.Add(stand);
        }
    }
}