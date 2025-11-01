using ReportEngine.App.Services.Interfaces;
using System.Windows;

namespace ReportEngine.App.Services;

public class NotificationService : INotificationService
{
    public void ShowError(string message)
    {
        MessageBox.Show(message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
    }

    public void ShowInfo(string message)
    {
        MessageBox.Show(message, "Информация", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    public bool ShowConfirmation(string message, string title = "Подтверждение")
    {
        return MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question) ==
               MessageBoxResult.Yes;
    }
}
