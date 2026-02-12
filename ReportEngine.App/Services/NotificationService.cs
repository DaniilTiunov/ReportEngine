using System.Windows;
using ReportEngine.App.Services.Interfaces;

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
        var owner = Application.Current.MainWindow;
        return MessageBox.Show(owner, message, title, MessageBoxButton.YesNo, MessageBoxImage.Question) ==
           MessageBoxResult.Yes;
    }
}
