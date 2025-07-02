using System.Windows;

namespace ReportEngine.App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void High_button_click(object sender, RoutedEventArgs e)
        {
            textDisplay.Content = "HMI quality is so high";
        }

        private void Low_button_click(object sender, RoutedEventArgs e)
        {
            textDisplay.Content = "HMI quality is so low";
        }

        private void HighQualityButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}