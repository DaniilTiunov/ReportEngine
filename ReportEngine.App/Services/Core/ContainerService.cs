using ReportEngine.App.Services.Interfaces;

namespace ReportEngine.App.Services.Core;

public class ContainerService
{
    private readonly INotificationService _notificationService;

    public ContainerService(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }


}
