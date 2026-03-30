using System.IO;
using System.Windows.Controls;
using ReportEngine.App.ViewModels;

namespace ReportEngine.App.Views.Controls;

/// <summary>
///     Логика взаимодействия для DockViewerView.xaml
/// </summary>
public partial class DockViewerView : UserControl
{
    private readonly DockViewerViewModel _viewModel;

    public DockViewerView(DockViewerViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;

        DataContext = _viewModel;

        _viewModel.OnFileOpenRequested += OpenFile;
        _viewModel.OnFileSaveRequested += SaveFile;
    }

    private void OpenFile(string path)
    {
        using (var stream = new FileStream(path, FileMode.Open, FileAccess.Read))
        {
            MainSreadSheet.Open(stream);
        }
    }

    private void SaveFile(string path)
    {
        using (var stream = new FileStream(path, FileMode.Create, FileAccess.Write))
        {
            MainSreadSheet.SaveAs(stream);
        }
    }
}
