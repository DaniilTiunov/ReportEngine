using System.Windows;
using ReportEngine.App.ViewModels.FormedEquips;

namespace ReportEngine.App.Views.Windows;

/// <summary>
///     Логика взаимодействия для FormedDrainagesView.xaml
/// </summary>
public partial class FormedDrainagesView : Window
{
    public FormedDrainagesView(FormedDrainagesViewModel formedDrainagesViewModel)
    {
        InitializeComponent();
        DataContext = formedDrainagesViewModel;
    }
}