using System.Windows.Controls;
using MahApps.Metro.Controls;
using ReportEngine.App.Services.Interfaces;
using ReportEngine.App.ViewModels;

namespace ReportEngine.App.Views.Windows.Dialog;

/// <summary>
///     Логика взаимодействия для StandsSettingsView.xaml
/// </summary>
public partial class StandsSettingsView : MetroWindow, IWindowWithViewModel<ProjectViewModel>
{
    private readonly ProjectViewModel _projectViewModel;

    public ProjectViewModel ViewModel { get; }
    
    public StandsSettingsView(ProjectViewModel projectViewModel)
    {
        InitializeComponent();
        _projectViewModel = projectViewModel;
        ViewModel = projectViewModel;
    }

    private async void StandsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        await _projectViewModel.OnFillStandFieldsFromSelectedStandCommandExecuted();
    }

}