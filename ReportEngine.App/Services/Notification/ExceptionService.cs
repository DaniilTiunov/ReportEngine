using ReportEngine.App.Services.Interfaces;
using ReportEngine.App.Services.Logger;

namespace ReportEngine.App.Services.Notification;

public class ExceptionService
{
    private readonly UiLogger _logger;
    private readonly INotificationService _notificationService;

    public ExceptionService(
        INotificationService notificationService,
        UiLogger logger
    )
    {
        _notificationService = notificationService;
        _logger = logger;
    }

    public void SafeExecute(Action action)
    {
        try
        {
            action();
        }
        catch (Exception ex)
        {
            _notificationService.ShowError(ex.Message);
            _logger.Error(ex.Message);
        }
    }

    public async Task SafeExecuteAsync(Func<Task> action)
    {
        try
        {
            await action();
        }
        catch (Exception ex)
        {
            _notificationService.ShowError(ex.Message);
            _logger.Error(ex.Message);
        }
    }
}
