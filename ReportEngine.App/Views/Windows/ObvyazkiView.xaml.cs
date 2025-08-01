using ReportEngine.App.ViewModels;
using System.Windows;

namespace ReportEngine.App.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для ObvyazkiView.xaml
    /// </summary>
    public partial class ObvyazkiView : Window
    {
        public ObvyazkiView(ObvyazkaViewModel obvyazkiViewModel)
        {
            InitializeComponent();
            DataContext = obvyazkiViewModel;

            obvyazkiViewModel.OnShowAllObvyazkiCommandExecuted(null);
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }
    }
}
