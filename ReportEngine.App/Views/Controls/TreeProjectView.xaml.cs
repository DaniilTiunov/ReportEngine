using System.Windows;
using System.Windows.Controls;

namespace ReportEngine.App.Views.Controls
{
    /// <summary>
    /// Логика взаимодействия для TreeProjectView.xaml
    /// </summary>
    public partial class TreeProjectView : UserControl
    {
        public TreeProjectView()
        {
            InitializeComponent();
        }

        private void OpenCurrentView(object sender, RoutedEventArgs e)
        {
            try
            {
                LoadContentByTag(MainTree.SelectedItem.ToString());

            }
            catch(Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void LoadContentByTag(string tag)
        {
            switch (tag)
            {
                case "Карточка проекта":
                    MainContent.Content = new ProjectCardView();
                    break;
            }
        }
    }
}

