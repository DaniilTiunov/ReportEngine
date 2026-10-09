using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using MahApps.Metro.Controls;
using ReportEngine.App.Services.Interfaces;
using ReportEngine.App.ViewModels;

namespace ReportEngine.App.Views.Windows;

public partial class AllSortamentsView : MetroWindow, IWindowWithViewModel<AllSortamentsViewModel>
{
    private readonly bool _isDialog;
    private readonly AllSortamentsViewModel _viewModel;
    private ICollectionView? _equipView;
    
    public AllSortamentsViewModel ViewModel { get; }
    

    public AllSortamentsView(
        AllSortamentsViewModel viewModel,
        bool isDialog = false)
    {
        InitializeComponent();
        DataContext = viewModel;
        ViewModel = viewModel;
        _viewModel = viewModel;
        _isDialog = isDialog;

        CheckIsDialog();
    }

    private void CheckIsDialog()
    {
        EquipDataGrid.IsReadOnly = _isDialog;
    }

    private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (_equipView == null)
            return;

        FinishPendingViewEdit();

        var query = SearchTextBox.Text.Trim();

        if (string.IsNullOrEmpty(query))
            _equipView.Filter = null;
        else
            _equipView.Filter = obj =>
            {
                var prop = obj?.GetType().GetProperty("Name");
                var value = prop?.GetValue(obj) as string;
                return !string.IsNullOrEmpty(value) &&
                       value.Contains(query, StringComparison.CurrentCultureIgnoreCase);
            };

        _equipView.Refresh();
    }

    private async void SubTabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.Source is not TabControl) return;
        if ((sender as TabControl)?.SelectedItem is not TabItem selectedTab) return;

        CancelPendingGridEdit();

        var groupKey = selectedTab.Tag as string;
        if (string.IsNullOrWhiteSpace(groupKey)) return;

        _viewModel.TabItemKey = groupKey;

        await _viewModel.LoadGroupAsync(groupKey);
        _viewModel.GenerateDataGridByTag(EquipDataGrid, groupKey);

        _viewModel.CurrentGroupKey = groupKey;

        if (!_viewModel.CurrentSortamentsModel.EquipGroups.TryGetValue(groupKey, out var collection))
            return;

        // У каждого окна должно быть своё представление коллекции. DefaultView является общим
        // и может сохранить EditItem-транзакцию после закрытия предыдущего окна.
        _equipView = new ListCollectionView(collection);
        EquipDataGrid.ItemsSource = _equipView;
    }

    private void CancelPendingGridEdit()
    {
        if (EquipDataGrid.IsReadOnly)
            return;

        EquipDataGrid.CancelEdit(DataGridEditingUnit.Cell);
        EquipDataGrid.CancelEdit(DataGridEditingUnit.Row);
    }

    private void FinishPendingViewEdit()
    {
        if (_equipView is not IEditableCollectionView editableView)
            return;

        if (editableView.IsAddingNew)
            editableView.CommitNew();
        if (editableView.IsEditingItem)
            editableView.CommitEdit();
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
