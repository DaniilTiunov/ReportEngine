using ReportEngine.App.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace ReportEngine.App.Views.Controls;

/// <summary>
///     Логика взаимодействия для StandObvView.xaml
/// </summary>
public partial class StandObvView : UserControl
{
    public StandObvView(ProjectViewModel projectViewModel)
    {
        InitializeComponent();
        DataContext = projectViewModel;

        Loaded += async (_, __) => await InitializeDataAsync(projectViewModel);
    }

    private async Task InitializeDataAsync(ProjectViewModel viewModel)
    {
        await viewModel.LoadStandsDataAsync();
        await viewModel.LoadObvyazkiAsync();
        await viewModel.LoadAllAvaileDataAsync();
        await viewModel.LoadPurposesInStandsAsync();
    }

    private void ComboBox_DropDownOpened(object sender, EventArgs e)
    {
        if (sender is ComboBox comboBox && comboBox.Template.FindName("PART_Popup", comboBox) is Popup popup)
            if (popup.Child is Border border)
                border.Background = (Brush)Application.Current.Resources["SecondaryColor"];
    }
}