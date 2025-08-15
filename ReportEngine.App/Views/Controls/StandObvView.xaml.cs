using System.Windows;
using ReportEngine.App.ViewModels;
using System.Windows.Controls;

namespace ReportEngine.App.Views.Controls
{
    /// <summary>
    /// Логика взаимодействия для StandObvView.xaml
    /// </summary>
    public partial class StandObvView : UserControl
    {
        public StandObvView(ProjectViewModel projectViewModel)
        {
            InitializeComponent();
            DataContext = projectViewModel;
        }
        private void StandObvView_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is ProjectViewModel viewModel)
            {
                viewModel.LoadObvyazkiAsync();
            }
        } 
    }
}
