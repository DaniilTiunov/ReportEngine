using ReportEngine.App.AppHelpers;
using ReportEngine.App.Model.StandsModel;
using ReportEngine.App.ViewModels;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;

namespace ReportEngine.App.Views.Controls;

/// <summary>
///     Логика взаимодействия для StandObvView.xaml
/// </summary>
public partial class StandObvView : UserControl
{
    private bool _allowEdit;

    private StandModel? _currentSelectedStand;
    private PropertyChangedEventHandler? _currentEventHandler;

    private readonly ProjectViewModel _projectViewModel;

    public StandObvView(ProjectViewModel projectViewModel)
    {
        InitializeComponent();
        DataContext = projectViewModel;

        _projectViewModel = projectViewModel;

        Loaded += async (_, __) => await InitializeDataAsync(projectViewModel);

    }

    private async Task InitializeDataAsync(ProjectViewModel projectViewModel)
    {
        await projectViewModel.LoadStandsDataAsync();
        await projectViewModel.LoadObvyazkiAsync();
        await projectViewModel.LoadAllAvaileDataAsync();
        await projectViewModel.LoadPurposesInStandsAsync();

        projectViewModel.UpdateNewObvNN();
        projectViewModel.UpdateChannelsQuantity();
        projectViewModel.UpdateCableInputsQuantity();
        projectViewModel.UpdateClampsQuantity();
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

    private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        //отписываемся от старого
        if (_currentSelectedStand != null && _currentEventHandler != null)
        {
            Debug.WriteLine("Отписываемся от старого стенда...");
            _currentSelectedStand.PropertyChanged -= _currentEventHandler;
            _currentEventHandler = null;
            Debug.WriteLine("Отписались от старого стенда");
        }

        //забираем новый выделенный элемент
        _currentSelectedStand = StandsList.SelectedItem as StandModel;

        ;
        //создаем новый обработчик
        if (_currentSelectedStand == null) 
        {
            Debug.WriteLine("Стенд не выбран, выход из обработчика");
            return;
        }
        ;
        Debug.WriteLine("Подписываемся  на новый стенд....");
        _currentEventHandler = (sender, e) =>
        {
            _projectViewModel.SubscribeData();
        };
        ;
        _currentSelectedStand.PropertyChanged += _currentEventHandler;
        Debug.WriteLine("Подписаны на новый стенд");

    }
}
