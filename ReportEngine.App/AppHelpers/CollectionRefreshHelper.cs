using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;

namespace ReportEngine.App.AppHelpers;

public static class CollectionRefreshHelper
{
    public static void SafeRefreshCollection(object collection)
    {
        try
        {
            var view = CollectionViewSource.GetDefaultView(collection);
            view?.Refresh();
        }
        catch (InvalidOperationException)
        {
            Application.Current.Dispatcher.BeginInvoke(
                DispatcherPriority.Background,
                new Action(() =>
                {
                    try
                    {
                        var view = CollectionViewSource.GetDefaultView(collection);
                        view?.Refresh();
                    }
                    catch
                    {
                    }
                }));
        }
    }
}
