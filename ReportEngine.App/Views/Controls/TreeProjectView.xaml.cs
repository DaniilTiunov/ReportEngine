using ReportEngine.App.ViewModels;
using ReportEngine.Shared.Config.DebugConsol;
using ReportEngine.Shared.Helpers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ReportEngine.App.Views.Controls
{
    public partial class TreeProjectView : UserControl, IDisposable
    {
        private bool _disposed;

        private ProjectViewModel _projectViewModel;
        public TreeProjectView(ProjectViewModel projectViewModel)
        {
            InitializeComponent();
            _projectViewModel = projectViewModel;
            DataContext = projectViewModel;
        }
        private void OpenCurrentView(object sender, MouseButtonEventArgs e)
        {
            ExceptionHelper.SafeExecute(() =>
            {
                var treeViewItem = MainTree.SelectedItem as TreeViewItem;
                if (treeViewItem?.Tag != null)
                {
                    string? header = treeViewItem.Header.ToString();
                    string? tag = treeViewItem.Tag.ToString();
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

                var tabControl = MainTabControl.Items
                                    .Add(new TabItem() { Header = header, Content = content });
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
        public void Dispose()
        {
            if (_disposed) return;

            DataContext = null;

            if (this.Resources != null)
            {
                this.Resources.Clear();
            }

            _disposed = true;
        }
        ~TreeProjectView()
        {
            Dispose();
        }
    }
}

