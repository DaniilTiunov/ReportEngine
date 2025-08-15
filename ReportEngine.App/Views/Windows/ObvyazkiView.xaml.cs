using System.ComponentModel;
using System.Windows;
using ReportEngine.App.ViewModels;

namespace ReportEngine.App.Views.Windows;

/// <summary>
///     Логика взаимодействия для ObvyazkiView.xaml
/// </summary>
public partial class ObvyazkiView : Window
{
    public ObvyazkiView(ObvyazkaViewModel obvyazkiViewModel)
    {
        InitializeComponent();
        DataContext = obvyazkiViewModel;

        obvyazkiViewModel.OnShowAllObvyazkiCommandExecuted(null);
    }

    private void Window_Closing(object sender, CancelEventArgs e)
    {
        e.Cancel = true;
        Hide();
    }
}