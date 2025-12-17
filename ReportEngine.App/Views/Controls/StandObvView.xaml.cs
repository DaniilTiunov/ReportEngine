using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;
using ReportEngine.App.AppHelpers;
using ReportEngine.App.ViewModels;

namespace ReportEngine.App.Views.Controls;

/// <summary>
///     Логика взаимодействия для StandObvView.xaml
/// </summary>
public partial class StandObvView : UserControl
{
    private bool _allowEdit;

    private readonly ProjectViewModel _projectViewModel;

    public StandObvView(ProjectViewModel projectViewModel)
    {
        InitializeComponent();
        DataContext = projectViewModel;

        _projectViewModel = projectViewModel;

        Loaded += async (_, __) => await InitializeDataAsync(projectViewModel);

        PreviewKeyDown += StandObvView_PreviewKeyDown;
    }

    private async Task InitializeDataAsync(ProjectViewModel projectViewModel)
    {
        await projectViewModel.LoadStandsDataAsync();
        await projectViewModel.LoadObvyazkiAsync();
        await projectViewModel.LoadAllAvaileDataAsync();
        await projectViewModel.LoadPurposesInStandsAsync();

        projectViewModel.OnObvyazkiInStandChanged();
        projectViewModel.OnFramesInStandChanged();
    }

    private async void StandObvView_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.F5)
        {
            e.Handled = true;

            await InitializeDataAsync(_projectViewModel);
        }
    }

    private void FillStandFieldsFromObvyazkaCommand_DoubleClick(object sender, MouseButtonEventArgs e)
    {
        ExceptionHelper.SafeExecute(() =>
        {
            _projectViewModel.OnFillStandFieldsFromObvyazkaCommandExecuted(sender);
        });
    }

    private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        var scrollViewer = sender as ScrollViewer;
        if (scrollViewer == null) return;

        // Принудительно прокручиваем ScrollViewer
        scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta);
        e.Handled = true;
    }

    private void DataGrid_BeginningEdit(object sender, DataGridBeginningEditEventArgs e)
    {
        if (!_allowEdit)
            e.Cancel = true;
    }

    private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (sender is not DataGrid grid)
            return;

        _allowEdit = true;

        if (grid.CurrentCell != null)
        {
            grid.BeginEdit();
        }

        _allowEdit = false;
    }

    private void StandsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        Debug.WriteLine("Стенд изменился");

        _projectViewModel.OnSelectedStandChanged();
    }
}
