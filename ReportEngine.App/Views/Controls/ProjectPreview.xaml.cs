using ReportEngine.App.ViewModels;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace ReportEngine.App.Views.Controls;

public partial class ProjectPreview : UserControl
{
    public ProjectPreview(ProjectViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;

        CommandBindings.Add(new CommandBinding(ApplicationCommands.Paste, OnPasteExecuted, OnPasteCanExecute));

        InitializeData(viewModel);
    }

    private async void InitializeData(ProjectViewModel viewModel)
    {
        await viewModel.LoadObvyazkiAsync();
        await viewModel.LoadStandsDataAsync();
        await viewModel.LoadPurposesInStandsAsync();
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
        if (DataContext is ProjectViewModel viewModel &&
            viewModel.CurrentProjectModel?.SelectedStand != null &&
            Clipboard.ContainsImage())
        {
            e.CanExecute = true;
            return;
        }

        e.CanExecute = false;
    }

    private async void OnPasteExecuted(object sender, ExecutedRoutedEventArgs e)
    {
        if (DataContext is not ProjectViewModel viewModel) return;
        var stand = viewModel.CurrentProjectModel?.SelectedStand;
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

            await viewModel.UpdateStandBlueprintAsync(bytes, "image/png");
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Не удалось вставить изображение: {ex.Message}", "Ошибка", MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
}