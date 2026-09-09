using System.Windows.Input;
using MahApps.Metro.Controls;
using ReportEngine.App.ViewModels;

namespace ReportEngine.App.Views.Windows.Dialog;

/// <summary>
///     Логика взаимодействия для AllStandsView.xaml
/// </summary>
public partial class AllStandsView : MetroWindow
{
    private readonly AllStandsViewModel _allStandsViewModel;

    public AllStandsView(AllStandsViewModel allStandsViewModel)
    {
        InitializeComponent();
        DataContext = allStandsViewModel;
    }

    private void StandsList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is AllStandsViewModel vm && vm.SelectedStand != null)
        {
            vm.ConfirmSelection();

            DialogResult = true;
        }
    }
}