using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using MahApps.Metro.Controls;
using ReportEngine.App.ViewModels;

namespace ReportEngine.App.Views.Windows;

public partial class AllSortamentsView : MetroWindow
{
    private readonly bool _isDialog;
    private readonly AllSortamentsViewModel _viewModel;
    private string _currentGroupKey;
    private ICollectionView _equipView;

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

        // Перед сменой данных отменяем текущее редактирование в DataGrid —
        // когда колонки/ItemsSource меняются во время редактирования.
        try
        {
            if (!EquipDataGrid.IsReadOnly) EquipDataGrid.CancelEdit(DataGridEditingUnit.Row);
        }
        catch
        {
        }

        ResetAllSubTabControls();

        var groupKey = selectedTab.Tag as string;
        if (string.IsNullOrWhiteSpace(groupKey)) return;

        _viewModel.TabItemKey = groupKey;

        await _viewModel.LoadGroupAsync(groupKey);
        _viewModel.TargetDataGrid = EquipDataGrid;
        _viewModel.GenerateDataGridByTag(EquipDataGrid, groupKey);

        _currentGroupKey = groupKey;
        _viewModel.CurrentGroupKey = groupKey;

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
    }
}