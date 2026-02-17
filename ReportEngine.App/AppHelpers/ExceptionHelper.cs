using System.Windows;
using ReportEngine.App.Enums;
using ReportEngine.App.Views.Windows.Dialog;

namespace ReportEngine.App.AppHelpers;

public static class ExceptionHelper
{
    public static void SafeExecute(Action action)
    {
        try
        {
            action();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    public static async Task SafeExecuteAsync(Func<Task> action)
    {
        try
        {
            await action();
        }
        catch (Exception ex)
        {
            ShowError(ex);
        }
    }

    private static void ShowError(Exception ex)
    {
        ShowNotification(ex.Message, "Ошибка", NotificationType.Error);
    }

    private static void ShowNotification(string message, string title, NotificationType type)
    {
        var window = new NotifyWindow(message, type, title);

        if (Application.Current?.MainWindow != window)
            window.Owner = Application.Current?.MainWindow;

        window.ShowDialog();
    }
}
