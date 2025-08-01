using System.Windows;

namespace ReportEngine.App.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для ObvyazkiView.xaml
    /// </summary>
    public partial class ObvyazkiView : Window
    {
        public ObvyazkiView(ObvyazkiViewModel obvyazkiViewModel)
        {
            InitializeComponent();
            DataContext = obvyazkiViewModel;
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }
    }
}
