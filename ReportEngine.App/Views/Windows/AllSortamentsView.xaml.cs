using System.Windows;
using System.Windows.Controls;
using ReportEngine.App.ViewModels;

namespace ReportEngine.App.Views.Windows;

public partial class AllSortamentsView : Window
{
    private readonly AllSortamentsViewModel _viewModel;

    public AllSortamentsView(AllSortamentsViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        _viewModel = viewModel;
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
    }
    
    
    // TODO: Исправить этот костыль
    private void ResetAllSubTabControls()
    {
        foreach (var mainTabItem in MainTabControl.Items)
        {
            if (mainTabItem is TabItem tabItem && tabItem.Content is TabControl subTabControl)
            {
                subTabControl.SelectedIndex = -1;
            }
        }
    }
}