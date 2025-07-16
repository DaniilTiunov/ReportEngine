using System.Windows.Controls;
using System.Windows.Input;

namespace ReportEngine.App.Views.Controls
{
    public partial class TreeProjectView : UserControl
    {
        public TreeProjectView()
        {
            InitializeComponent();
        }
        private void OpenCurrentView(object sender, MouseButtonEventArgs e)
        {
            var treeViewItem = MainTree.SelectedItem as TreeViewItem;
            if (treeViewItem?.Tag != null)
            {
                string tag = treeViewItem.Tag.ToString();
                LoadContent(tag);
            }
        }
        private void LoadContent(string tag)
        {
            if (string.IsNullOrEmpty(tag))
                return;

            UserControl content = tag switch
            {
                "ProjectCard" => new ProjectCardView()
            };

            MainContent.Content = content;
        }
    }
}

