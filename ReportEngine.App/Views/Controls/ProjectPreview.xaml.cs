using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using ReportEngine.App.Display;
using ReportEngine.App.ViewModels;

namespace ReportEngine.App.Views.Controls;

public partial class ProjectPreview : UserControl
{

    // TODO: Нужен фикс

    private bool _allowEdit;

    private readonly ProjectViewModel _projectViewModel;

    public ProjectPreview(ProjectViewModel projectViewModel)
    {
        InitializeComponent();
        DataContext = projectViewModel;
        _projectViewModel = projectViewModel;

        CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, OnPasteExecuted, OnPasteCanExecute));

        Loaded += async (_, __) => await InitializeDataAsync(_projectViewModel);

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
        projectViewModel.UpdateNewStandNN();
        projectViewModel.OnStandsInProjectChanged();
    }

    // Защита от повторного вызова
    private async void OnLoaded(object sender, RoutedEventArgs e)
    {
        Loaded -= OnLoaded;

        try
        {
            await InitializeDataAsync(_projectViewModel);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }
    // Защита от автоповтора F5
    private async void StandObvView_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.F5 || e.IsRepeat)
            return;

        e.Handled = true;

        try
        {
            await InitializeDataAsync(_projectViewModel);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    private void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        var scrollViewer = sender as ScrollViewer;
        if (scrollViewer == null) return;

        // Принудительно прокручиваем ScrollViewer
        scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta);
        e.Handled = true;
    }

    private void Image_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (sender is Control c)
        {
            c.Focus();
            e.Handled = true;
        }
    }

    private void OnPasteCanExecute(object sender, CanExecuteRoutedEventArgs e)
    {
        if (DataContext is ProjectViewModel projectViewModel &&
            projectViewModel.CurrentProjectModel?.SelectedStand != null &&
            Clipboard.ContainsImage())
        {
            e.CanExecute = true;
            return;
        }

        e.CanExecute = false;
    }

    private async void OnPasteExecuted(object sender, ExecutedRoutedEventArgs e)
    {
        if (DataContext is not ProjectViewModel projectViewModel) return;
        var stand = projectViewModel.CurrentProjectModel?.SelectedStand;
        if (stand == null) return;
        if (!Clipboard.ContainsImage()) return;

        var bitmap = Clipboard.GetImage();
        if (bitmap == null) return;

        try
        {
            byte[] bytes;
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            using (var memoryStream = new MemoryStream())
            {
                encoder.Save(memoryStream);
                bytes = memoryStream.ToArray();
            }

            stand.ImageData = bytes;
            stand.ImageType = "image/png";

            await projectViewModel.UpdateStandBlueprintAsync(bytes, "image/png");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Не удалось вставить изображение: {ex.Message}", "Ошибка", MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
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
        _projectViewModel.OnSelectedStandChanged();
    }
}
