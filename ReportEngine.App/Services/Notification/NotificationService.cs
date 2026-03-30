using System.Windows;
using ReportEngine.App.Enums;
using ReportEngine.App.Services.Interfaces;
using ReportEngine.App.Views.Windows.Dialog;

namespace ReportEngine.App.Services.Notification;

public class NotificationService : INotificationService
{
    public void ShowError(string message)
    {
        var window = new NotifyWindow(message, NotificationType.Error, "Ошибка")
        {
            Owner = Application.Current.MainWindow
        };
        window.ShowDialog();
    }

    public void ShowInfo(string message)
    {
        var window = new NotifyWindow(message, NotificationType.Info, "Информация")
        {
            Owner = Application.Current.MainWindow
        };
        window.ShowDialog();
    }

    public bool ShowConfirmation(string message, string title = "Подтверждение")
    {
        var window = new NotifyWindow(message, NotificationType.Confirmation, title)
        {
            Owner = Application.Current.MainWindow
        };
        var result = window.ShowDialog();
        return result == true;
    }
}
