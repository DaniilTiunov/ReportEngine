using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using ReportEngine.App.ViewModels;

namespace ReportEngine.App.Views.Controls;

/// <summary>
///     Логика взаимодействия для StandsContainerView.xaml
/// </summary>
public partial class StandsContainerView : UserControl
{
    private readonly ContainersViewModel _viewModel;
    private bool _allowEdit;

    public StandsContainerView(ContainersViewModel _viewModel)
    {
        DataContext = _viewModel;
        InitializeComponent();
    }

    private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
    {
        if (e.Row.Item != CollectionView.NewItemPlaceholder)
            return;

        var dataGrid = sender as DataGrid;
        if (dataGrid == null)
            return;

        e.Row.Dispatcher.BeginInvoke(new Action(() =>
        {
            var presenter = FindVisualChild<DataGridCellsPresenter>(e.Row);
            if (presenter == null)
                return;

            var columnCount = dataGrid.Columns.Count;

            for (var i = 0; i < columnCount; i++)
            {
                var cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(i);
                if (cell == null)
                    continue;

                if (i == 0)
                {
                    Grid.SetColumnSpan(cell, columnCount);

                    var button = new Button
                    {
                        Content = "➕",
                        Foreground = Brushes.White,
                        VerticalAlignment = VerticalAlignment.Center,
                        Cursor = Cursors.Hand
                    };

                    cell.Content = button;
                }
                else
                {
                    cell.Visibility = Visibility.Collapsed;
                }
            }
        }), DispatcherPriority.Loaded);
    }

    private static T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
    {
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
        {
            var child = VisualTreeHelper.GetChild(obj, i);

            if (child is T t)
                return t;

            var childOfChild = FindVisualChild<T>(child);
            if (childOfChild != null)
                return childOfChild;
        }

        return null;
    }

    private void DataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
    {
        if (!_allowEdit)
            e.Cancel = true;
    }

    private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (sender is not DataGrid grid)
            return;

        _allowEdit = true;

        if (grid.CurrentCell != null) grid.BeginEdit();

        _allowEdit = false;
    }
}
