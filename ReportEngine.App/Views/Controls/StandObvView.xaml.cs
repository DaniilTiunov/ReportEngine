using System.Windows;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore.Diagnostics;
using ReportEngine.App.Services;
using ReportEngine.App.ViewModels;
using ReportEngine.App.Views.Windows;

namespace ReportEngine.App.Views.Controls;

/// <summary>
///     Логика взаимодействия для StandObvView.xaml
/// </summary>
public partial class StandObvView : UserControl
{
    public StandObvView(ProjectViewModel projectViewModel)
    {
        InitializeComponent();
        DataContext = projectViewModel;
        
        Loaded += StandObvView_Loaded;
    }
    
    private void StandObvView_Loaded(object sender, RoutedEventArgs e)
    {
        if (DataContext is ProjectViewModel viewModel) viewModel.LoadObvyazkiAsync();
    }
}