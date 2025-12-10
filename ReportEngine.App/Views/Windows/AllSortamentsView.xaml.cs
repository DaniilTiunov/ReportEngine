using ReportEngine.App.ViewModels;
using ReportEngine.App.ViewModels.Contacts;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace ReportEngine.App.Views.Windows;

public partial class AllSortamentsView : Window
{
    private ICollectionView _equipView;

    private readonly AllSortamentsViewModel _viewModel;

    private readonly bool _isDialog;
    public AllSortamentsView(AllSortamentsViewModel viewModel, bool isDialog = false)
    {
        InitializeComponent();
        DataContext = viewModel;
        _viewModel = viewModel;
        _isDialog = isDialog;
    }
    private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (_equipView == null)
            return;

        var query = SearchTextBox.Text.Trim().ToLower();

        if (string.IsNullOrEmpty(query))
            _equipView.Filter = null;
        else
            _equipView.Filter = obj =>
            {
                // Пример фильтрации по свойству Name
                var prop = obj?.GetType().GetProperty("Name");
                var value = prop?.GetValue(obj) as string;
                EquipDataGrid.ItemsSource = _equipView;
                return !string.IsNullOrEmpty(value) && value.ToLower().Contains(query);
            };

        _equipView.Refresh();
    }

    private async void SubTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.Source is not TabControl) return;
        if ((sender as TabControl)?.SelectedItem is not TabItem selectedTab) return;

        ResetAllSubTabControls();

        var groupKey = selectedTab.Tag as string;
        if (string.IsNullOrWhiteSpace(groupKey)) return;

        await _viewModel.LoadGroupAsync(groupKey);
        _viewModel.GenerateDataGridByTag(EquipDataGrid, groupKey);

        if (_viewModel.CurrentSortamentsModel.EquipGroups.TryGetValue(groupKey, out var collection))
            EquipDataGrid.ItemsSource = collection;
            _equipView = CollectionViewSource.GetDefaultView(collection);
    }

    // TODO: Исправить этот костыль
    private void ResetAllSubTabControls()
    {
        foreach (var mainTabItem in MainTabControl.Items)
            if (mainTabItem is TabItem tabItem && tabItem.Content is TabControl subTabControl)
                subTabControl.SelectedIndex = -1;
    }

    private void SelectEquip_DoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (_viewModel.SelectedEquip != null && _isDialog)
        {
            _viewModel.SelectionHandler?.Invoke(_viewModel.SelectedEquip);
            Close();
        }
        else
        {
            Close();
        }
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
            Width = 1100;
            Height = 450;
            Left = (SystemParameters.PrimaryScreenWidth - Width) / 2;
            Top = (SystemParameters.PrimaryScreenHeight - Height) / 2;
        }
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}
