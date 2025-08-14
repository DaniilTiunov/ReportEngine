namespace ReportEngine.Services.Interfaces;

public interface INotificationService
{
    void ShowInfo(string message);
    void ShowWarning(string message);
    void ShowError(string message);
    bool ShowConfirmation(string message, string title = "Подтверждение");
}