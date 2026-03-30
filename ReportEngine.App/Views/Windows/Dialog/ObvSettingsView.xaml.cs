using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace ReportEngine.App.Views.Windows.Dialog;

/// <summary>
///     Логика взаимодействия для ObvSettingsView.xaml
/// </summary>
public partial class ObvSettingsView : Window
{
    private bool _allowEdit;

    public ObvSettingsView()
    {
        InitializeComponent();
    }

    private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2)
            MaxRestoreButton_Click(sender, e);
        else
            DragMove();
    }

    private void MinimizeButton_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void MaxRestoreButton_Click(object sender, RoutedEventArgs e)
    {
        var area = SystemParameters.WorkArea;
        if (Width != area.Width || Height != area.Height || Left != area.Left || Top != area.Top)
        {
            Left = area.Left;
            Top = area.Top;
            Width = area.Width;
            Height = area.Height;
        }
        else
        {
            Width = 1280;
            Height = 800;
            Left = (SystemParameters.PrimaryScreenWidth - Width) / 2;
            Top = (SystemParameters.PrimaryScreenHeight - Height) / 2;
        }
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
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

    private void ObvyazkiDataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
    {
        if (e.Row.Item != CollectionView.NewItemPlaceholder)
            return;

        e.Row.Dispatcher.BeginInvoke(new Action(() =>
        {
            var presenter = FindVisualChild<DataGridCellsPresenter>(e.Row);
            if (presenter == null)
                return;

            var columnCount = ObvyazkiDataGrid.Columns.Count;

            for (var i = 0; i < columnCount; i++)
            {
                var cell = (DataGridCell)presenter.ItemContainerGenerator.ContainerFromIndex(i);
                if (cell == null)
                    continue;

                if (i == 0)
                {
                    Grid.SetColumnSpan(cell, columnCount);
                    cell.Content = new Button
                    {
                        Content = "➕",
                        Foreground = Brushes.White,
                        VerticalAlignment = VerticalAlignment.Center,
                        Cursor = Cursors.Hand
                    };
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
}
