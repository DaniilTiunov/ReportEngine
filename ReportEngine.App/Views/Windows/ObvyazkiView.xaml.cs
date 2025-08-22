using System.ComponentModel;
using System.Windows;
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

    private void ObvyazkaListBox_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is ObvyazkaViewModel vm && vm.CurrentObvyazka.SelectedObvyazka != null)
        {
            vm.SelectionHandler?.Invoke(vm.CurrentObvyazka.SelectedObvyazka);
            Close();
        }
    }

    private void InitializeData(ObvyazkaViewModel obvyazkiViewModel)
    {
        obvyazkiViewModel.ShowAllObvyazkiAsync();
    }

    private void Window_Closing(object sender, CancelEventArgs e)
    {
        e.Cancel = true;
        Hide();
    }
}