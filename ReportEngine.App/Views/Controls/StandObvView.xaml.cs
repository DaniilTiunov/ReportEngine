using ReportEngine.App.AppHelpers;
using ReportEngine.App.ViewModels;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Input;

namespace ReportEngine.App.Views.Controls;

/// <summary>
///     Логика взаимодействия для StandObvView.xaml
/// </summary>
public partial class StandObvView : UserControl
{
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
    }

    private void FillStandFieldsFromObvyazkaCommand_DoubleClick(object sender, MouseButtonEventArgs e)
    {
        ExceptionHelper.SafeExecute(() =>
        {
            _projectViewModel.OnFillStandFieldsFromObvyazkaCommandExecuted(sender);
        });
    }

    private void TypeSensor_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        ExceptionHelper.SafeExecute(() =>
        {

            var electicComponents = _projectViewModel.CurrentStandModel.NewElectricalComponent.Purposes;
            var cableInputsRecord = electicComponents
                .FirstOrDefault(purpose => purpose.Purpose == "Кабельные вводы");

            var sensorsQuantity = _projectViewModel.CurrentStandModel.CountSensorsQuantity();
            if (cableInputsRecord != null)
            {
                //TODO:забыть как страшный сон, временное решение
                var cableInputsQuantity = 0.5 * Math.Pow(sensorsQuantity, 3) - 1.5 * Math.Pow(sensorsQuantity, 2) + 3 * sensorsQuantity;
                cableInputsRecord.Quantity = (float?) cableInputsQuantity;
            }

            CollectionRefreshHelper.SafeRefreshCollection(_projectViewModel.CurrentStandModel.NewElectricalComponent.Purposes);

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

}
