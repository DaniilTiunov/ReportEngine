using System.ComponentModel;
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

    public static void SafeSortAndRefreshCollection(object collection, string fieldToSortBy, bool descending)
    {
        var sortType = descending ? ListSortDirection.Descending : ListSortDirection.Ascending;
        try
        {
            var view = CollectionViewSource.GetDefaultView(collection);
            view.SortDescriptions.Clear();
            view.SortDescriptions.Add(
                new SortDescription(fieldToSortBy, sortType));
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
                        view.SortDescriptions.Clear();
                        view.SortDescriptions.Add(
                            new SortDescription(fieldToSortBy, sortType));
                        view?.Refresh();
                    }
                    catch
                    {
                    }
                }));
        }
    }
}
