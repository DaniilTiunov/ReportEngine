using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.DependencyInjection;
using ReportEngine.App.ViewModels;
using ReportEngine.App.Views;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;
using System.Diagnostics;
using System.Windows;

namespace ReportEngine.App
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IBaseRepository<User> _userRepository;

        public MainWindow(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _userRepository = _serviceProvider.GetRequiredService<IBaseRepository<User>>();

            InitializeComponent();
            InitializeDataContext(); 
        }
        private void InitializeDataContext()
        {
            DataContext = new MainWindowViewModel(_userRepository, _serviceProvider);
        }
        private void ShowAboutProgram(object sender, RoutedEventArgs e)
        {
            var aboutWindow = new AboutProgram();
            
            aboutWindow.Show();
        }
        private void ShowCalculator(object sender, RoutedEventArgs e)
        {
            Process.Start("calc.exe");
        }
        private void ShowNotepad(object sender, RoutedEventArgs e)
        {
            Process.Start("notepad.exe");
        }
    }
}