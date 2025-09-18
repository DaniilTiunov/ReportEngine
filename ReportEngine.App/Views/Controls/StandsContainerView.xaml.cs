using ReportEngine.App.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ReportEngine.App.Views.Controls
{
    /// <summary>
    /// Логика взаимодействия для StandsContainerView.xaml
    /// </summary>
    public partial class StandsContainerView : UserControl
    {
        private readonly ProjectViewModel _projectViewModel;

        public StandsContainerView(ProjectViewModel projectViewModel)
        {
            _projectViewModel = projectViewModel;

            InitializeComponent();
            DataContext = projectViewModel;

            Loaded += StandsContainerView_Loaded;
        }

        private void StandsContainerView_Loaded(object sender, RoutedEventArgs e)
        {
            _projectViewModel.LoadContainersInfoAsync();
        }
    }
}
