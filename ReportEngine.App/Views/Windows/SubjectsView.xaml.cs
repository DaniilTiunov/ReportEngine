using ReportEngine.App.ViewModels.Contacts;
using ReportEngine.Domain.Entities;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace ReportEngine.App.Views.Windows;

/// <summary>
///     Логика взаимодействия для CompanyView.xaml
/// </summary>
public partial class SubjectsView : Window
{
    private ICollectionView _subjectsView;

    public SubjectsView(SubjectViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;

        Loaded += async (_, __) => await InitializeDataAsync(viewModel);
    }

    private async Task InitializeDataAsync(SubjectViewModel viewModel)
    {
        await viewModel.LoadAllSubjectsAsync();

        _subjectsView = CollectionViewSource.GetDefaultView(viewModel.CurrentSubject.AllSubjects);

        SubjectsDataGrid.ItemsSource = _subjectsView;
    }

    private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (_subjectsView == null)
            return;

        var query = SearchTextBox.Text.Trim().ToLower();

        if (string.IsNullOrEmpty(query))
            _subjectsView.Filter = null; // сброс фильтра
        else
            _subjectsView.Filter = obj =>
            {
                if (obj is Subject c)
                    return !string.IsNullOrEmpty(c.ObjectName) && c.ObjectName.ToLower().Contains(query);
                return false;
            };

        _subjectsView.Refresh();
    }

    private void SubjectsDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is SubjectViewModel vm && vm.CurrentSubject.SelectedSubject != null)
        {
            vm.SelectedItem?.Invoke(vm.CurrentSubject.SelectedSubject.ObjectName);
            DialogResult = true;
            Close();
        }
    }

    private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2)
            MaxRestoreButton_Click(sender, e);
        else
            DragMove();
    }

    private void MinimizeButton_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void MaxRestoreButton_Click(object sender, RoutedEventArgs e)
    {
        var area = SystemParameters.WorkArea;
        if (Width != area.Width || Height != area.Height || Left != area.Left || Top != area.Top)
        {
            Left = area.Left;
            Top = area.Top;
            Width = area.Width;
            Height = area.Height;
        }
        else
        {
            Width = 1000;
            Height = 600;
            Left = (SystemParameters.PrimaryScreenWidth - Width) / 2;
            Top = (SystemParameters.PrimaryScreenHeight - Height) / 2;
        }
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }
}