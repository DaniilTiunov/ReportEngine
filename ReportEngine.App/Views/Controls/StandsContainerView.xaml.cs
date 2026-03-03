using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using ReportEngine.App.ViewModels;

namespace ReportEngine.App.Views.Controls;

/// <summary>
///     Логика взаимодействия для StandsContainerView.xaml
/// </summary>
public partial class StandsContainerView : UserControl
{
    private bool _allowEdit;

    private readonly ProjectViewModel _projectViewModel;

    public StandsContainerView(ProjectViewModel projectViewModel)
    {
        _projectViewModel = projectViewModel;

        InitializeComponent();
        DataContext = projectViewModel;

        Loaded += StandsContainerView_Loaded;
    }

    private async void StandsContainerView_Loaded(object sender, RoutedEventArgs e)
    {
        await _projectViewModel.LoadContainersInfoAsync();
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

            int columnCount = dataGrid.Columns.Count;

            for (int i = 0; i < columnCount; i++)
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
        }), System.Windows.Threading.DispatcherPriority.Loaded);
    }

    private static T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
    {
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
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

        if (grid.CurrentCell != null)
        {
            grid.BeginEdit();
        }

        _allowEdit = false;
    }
}
