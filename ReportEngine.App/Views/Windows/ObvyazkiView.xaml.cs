using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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

        InitializeData(obvyazkiViewModel);
    }
    private async void InitializeData(ObvyazkaViewModel obvyazkiViewModel)
    {
        await obvyazkiViewModel.ShowAllObvyazkiAsync();
    }
    private void Window_Closing(object sender, CancelEventArgs e)
    {
        e.Cancel = true;
        Hide();
    }
}