using ReportEngine.App.Services.Interfaces;
using ReportEngine.Domain.Repositories.Interfaces;

namespace ReportEngine.Services.Services;

public class ProjectService
{
    private readonly IProjectInfoRepository _projectRepository;
    private readonly IFrameRepository _formedFrameRepository;
    private readonly INotificationService _notificationService;
    private readonly IFormedDrainagesRepository _formedDrainagesRepository;
    private readonly IDialogService _dialogService;
    
    public ProjectService(IProjectInfoRepository projectRepository,
        IDialogService dialogService,
        INotificationService notificationService,
        IFrameRepository formedFrameRepository,
        IFormedDrainagesRepository formedDrainagesRepository)
    {
        _projectRepository = projectRepository;
        _dialogService = dialogService;
        _formedFrameRepository = formedFrameRepository;
        _formedDrainagesRepository = formedDrainagesRepository;
        _notificationService = notificationService;
    }
}