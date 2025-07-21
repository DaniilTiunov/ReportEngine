using ReportEngine.App.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ReportEngine.App.Views.Controls
{
    public partial class TreeProjectView : UserControl
    {
        private ProjectViewModel _projectViewModel;
        public TreeProjectView(ProjectViewModel projectViewModel)
        {
            InitializeComponent();
            _projectViewModel = projectViewModel;
            DataContext = projectViewModel;
        }
        private void OpenCurrentView(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var treeViewItem = MainTree.SelectedItem as TreeViewItem;
                if (treeViewItem?.Tag != null)
                {
                    string? header = treeViewItem.Header.ToString();
                    string? tag = treeViewItem.Tag.ToString();
                    LoadTreeContent(tag, header);
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }
        private void LoadTreeContent(string tag, string header)
        {
            try
            {
                if (string.IsNullOrEmpty(tag))
                    return;

                var content = CreateCurrentContent(tag);

                var tabControl = MainTabControl.Items
                                    .Add(new TabItem() { Header = header, Content = content });


            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
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
                };

                return control;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return null;
            }
        }
    }
}

