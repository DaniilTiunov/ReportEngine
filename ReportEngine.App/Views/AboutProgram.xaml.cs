using ReportEngine.App.ViewModels;
using System.Windows;
namespace ReportEngine.App.Views
{
    /// <summary>
    /// Логика взаимодействия для AboutProgram.xaml
    /// </summary>
    public partial class AboutProgram : Window
    {
        public AboutProgram()
        {
            InitializeComponent();
            DataContext = new AboutProgramViewModel(); // Устанавливаем DataContext
        }
    }
}
