using System.Windows;
using ReportEngine.App.ViewModels.FormedEquips;

namespace ReportEngine.App.Views.Windows;

/// <summary>
///     Логика взаимодействия для FormedFrameView.xaml
/// </summary>
public partial class FormedFrameView : Window
{
    private readonly FormedFrameViewModel _viewModel;

    public FormedFrameView(FormedFrameViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}