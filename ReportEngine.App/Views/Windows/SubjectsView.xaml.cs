using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using MahApps.Metro.Controls;
using ReportEngine.App.ViewModels.Contacts;
using ReportEngine.Domain.Entities;

namespace ReportEngine.App.Views.Windows;

/// <summary>
///     Логика взаимодействия для CompanyView.xaml
/// </summary>
public partial class SubjectsView : MetroWindow
{
    private readonly bool _isDialog;
    private ICollectionView _subjectsView;

    public SubjectsView(SubjectViewModel viewModel, bool isDialog = false)
    {
        InitializeComponent();
        DataContext = viewModel;

        _isDialog = isDialog;

        Loaded += async (_, __) => await InitializeDataAsync(viewModel);
        _isDialog = isDialog;
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
        if (DataContext is SubjectViewModel vm && vm.CurrentSubject.SelectedSubject != null && _isDialog)
        {
            vm.SelectedItem?.Invoke(vm.CurrentSubject.SelectedSubject.ObjectName);
            DialogResult = true;
            Close();
        }
    }
}