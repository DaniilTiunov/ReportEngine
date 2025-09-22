using System.Windows;
using System.Windows.Threading;
using ReportEngine.App.ViewModels;

namespace ReportEngine.App.AppHelpers;

public static class PropertyRefreshHelper
{
    public static void RefreshProperty(BaseViewModel vm, string propertyName)
    {
        try
        {
            vm?.OnPropertyChanged(propertyName);
        }
        catch (InvalidOperationException)
        {
            Application.Current?.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new Action(() =>
                {
                    try
                    {
                        vm?.OnPropertyChanged(propertyName);
                    }
                    catch
                    {
                    }
                }));
        }
    }
}