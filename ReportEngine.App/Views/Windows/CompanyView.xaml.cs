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
public partial class CompanyView : Window
{
    private ICollectionView _companiesView;

    private readonly bool _isDialog;

    public CompanyView(CompanyViewModel viewModel, bool IsDialog = false)
    {
        InitializeComponent();
        DataContext = viewModel;
        _isDialog = IsDialog;

        Loaded += async (_, __) => await InitializeDataAsync(viewModel);
    }

    private async Task InitializeDataAsync(CompanyViewModel viewModel)
    {
        await viewModel.LoadAllCompaniesAsync();

        _companiesView = CollectionViewSource.GetDefaultView(viewModel.CurrentCompany.AllCompanies);

        CompaniesDataGrid.ItemsSource = _companiesView;
    }

    private void CompaniesDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (DataContext is CompanyViewModel vm && vm.CurrentCompany.SelectedCompany != null && _isDialog)
        {
            vm.SelectedItem?.Invoke(vm.CurrentCompany.SelectedCompany.Name);
            DialogResult = true;
            Close();
        }
    }

    private void SearchTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (_companiesView == null)
            return;

        var query = SearchTextBox.Text.Trim().ToLower();

        if (string.IsNullOrEmpty(query))
            _companiesView.Filter = null; // сброс фильтра
        else
            _companiesView.Filter = obj =>
            {
                if (obj is Company c)
                    return !string.IsNullOrEmpty(c.Name) && c.Name.ToLower().Contains(query);
                return false;
            };

        _companiesView.Refresh();
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
