using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MahApps.Metro.Controls;
using ReportEngine.App.ViewModels;

namespace ReportEngine.App.Views.Windows;

/// <summary>
///     Логика взаимодействия для ObvyazkiView.xaml
/// </summary>
public partial class ObvyazkiView : MetroWindow
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

    private void ListBox_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        var scrollViewer = FindParent<ScrollViewer>(sender as DependencyObject);
        if (scrollViewer != null)
        {
            scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta);
            e.Handled = true;
        }
    }

    private static T FindParent<T>(DependencyObject child) where T : DependencyObject
    {
        while (child != null)
        {
            if (child is T parent)
                return parent;
            child = VisualTreeHelper.GetParent(child);
        }

        return null;
    }
}