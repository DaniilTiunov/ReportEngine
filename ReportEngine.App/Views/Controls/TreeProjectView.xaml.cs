using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ReportEngine.App.ViewModels;
using ReportEngine.Shared.Config.DebugConsol;
using ReportEngine.Shared.Helpers;

namespace ReportEngine.App.Views.Controls;

public partial class TreeProjectView : UserControl, IDisposable
{
    private readonly ProjectViewModel _projectViewModel;
    private bool _disposed;

    public TreeProjectView(ProjectViewModel projectViewModel)
    {
        InitializeComponent();
        _projectViewModel = projectViewModel;
        DataContext = projectViewModel;
    }

    public void Dispose()
    {
        if (_disposed) return;

        DataContext = null;

        if (Resources != null) Resources.Clear();

        _disposed = true;
    }

    private void OpenCurrentView(object sender, MouseButtonEventArgs e)
    {
        ExceptionHelper.SafeExecute(() =>
        {
            var treeViewItem = MainTree.SelectedItem as TreeViewItem;
            if (treeViewItem?.Tag != null)
            {
                var header = treeViewItem.Header.ToString();
                var tag = treeViewItem.Tag.ToString();
                LoadTreeContent(tag, header);
            }
        });
    }

    private void LoadTreeContent(string tag, string header)
    {
        ExceptionHelper.SafeExecute(() =>
        {
            if (string.IsNullOrEmpty(tag))
                return;

            var content = CreateCurrentContent(tag);
            if (content == null)
                return; // Не добавлять вкладку, если контент не создан

            var tabControl = new TabItem
            {
                Header = header,
                Content = content
            };

            MainTabControl.Items.Add(tabControl);
            MainTabControl.SelectedItem = tabControl;
        });
    }

    private UserControl CreateCurrentContent(string tag)
    {
        try
        {
            if (string.IsNullOrEmpty(tag))
                return null;

            UserControl control = tag switch
            {
                "ProjectCard" => new ProjectCardView(_projectViewModel),
                "Stand" => new StandView(_projectViewModel),
                "StandObv" => new StandObvView(_projectViewModel),
                "FrameDrainages" => new FrameDrainagesView(_projectViewModel)
            };

            return control;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
            DebugConsole.WriteLine(ex, ConsoleColor.Red);
            return null;
        }
    }

    ~TreeProjectView()
    {
        Dispose();
    }
}