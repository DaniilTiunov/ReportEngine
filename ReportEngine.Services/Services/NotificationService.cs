using ReportEngine.Services.Interfaces;

namespace ReportEngine.Services.Services;

public class NotificationService : INotificationService
{
    public bool ShowConfirmation(string message, string title = "Подтверждение")
    {
        throw new NotImplementedException();
    }

    public void ShowError(string message)
    {
        throw new NotImplementedException();
    }

    public void ShowInfo(string message)
    {
        throw new NotImplementedException();
    }

    public void ShowWarning(string message)
    {
        throw new NotImplementedException();
    }
}
