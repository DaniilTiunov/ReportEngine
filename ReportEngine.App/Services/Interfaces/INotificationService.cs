namespace ReportEngine.App.Services.Interfaces;

public interface INotificationService
{
    void ShowError(string message);
    void ShowInfo(string message);
    bool ShowConfirmation(string message, string title = "Подтверждение");
}