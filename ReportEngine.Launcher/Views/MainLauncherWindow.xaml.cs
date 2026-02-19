using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using ReportEngine.Shared.Config.Directory;
using ReportEngine.Shared.Config.JsonHelpers;

namespace ReportEngine.Launcher.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string Version => JsonHandler.GetCurrentVersion(DirectoryHelper.GetConfigPath());

        public MainWindow()
        {
            StandardTheme(null, null);

            DataContext = this;

            InitializeComponent();
        }

        private void StartStandardApp(object sender, RoutedEventArgs e)
        {
            try
            {
                var localPath = AppDomain.CurrentDomain.BaseDirectory;
                var appPath = Path.Combine(localPath, "ReportEngine.App.exe");

                if (!File.Exists(appPath))
                {
                    MessageBox.Show("Приложение 'Стенды КИПиА' не найдено!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                Process.Start(appPath);

                // Завершаем текущий WPF
                Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка запуска приложения: {ex.Message}");
            }
        }

        private void UpdateApp(object sender, RoutedEventArgs e)
        {
            try
            {
                var localPath = AppDomain.CurrentDomain.BaseDirectory;
                var updaterPath = Path.Combine(localPath, "ReportUpdater.exe");

                if (!File.Exists(updaterPath))
                {
                    MessageBox.Show("ReportUpdater.exe не найден!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                Process.Start(updaterPath);

                ShutDownApplications();

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка запуска обновления: {ex.Message}");
            }
        }

        private void ShutDownApplications()
        {
            foreach (var procName in new[] { "ReportEngine.App", "ReportEngine.AtomicApp" })
            {
                var processes = Process.GetProcessesByName(procName);
                foreach (var proc in processes)
                {
                    try
                    {
                        if (!proc.HasExited)
                        {
                            proc.CloseMainWindow();
                            proc.WaitForExit(2000);
                            if (!proc.HasExited)
                                proc.Kill();
                        }
                    }
                    catch
                    {
                        
                    }
                }
            }

            Application.Current.Shutdown();
        }

        private void StartAtomicApp(object sender, RoutedEventArgs e)
        {
            try
            {
                var localPath = AppDomain.CurrentDomain.BaseDirectory;
                var appPath = Path.Combine(localPath, "ReportEngine.AtomicApp.exe");

                if (!File.Exists(appPath))
                {
                    MessageBox.Show("Приложение 'АтомСтенды КИПиА' не найдено!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                Process.Start(appPath);

                // Завершаем текущий WPF
                Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка запуска приложения: {ex.Message}");
            }
        }

        private void StandardTheme(object sender, RoutedEventArgs e)
        {
            ChangesTheme("Resources/Dictionaries/ColorThemes/LightTheme.xaml");
        }

        private void ChangesTheme(string dictPath)
        {
            var uri = new Uri(dictPath, UriKind.Relative);
            var themeDict = Application.LoadComponent(uri) as ResourceDictionary;

            var mergedDicts = Application.Current.Resources.MergedDictionaries;
            for (int i = 0; i < mergedDicts.Count; i++)
            {
                if (mergedDicts[i].Source != null && mergedDicts[i].Source.OriginalString.Contains("ColorThemes"))
                {
                    mergedDicts[i] = themeDict;
                    return;
                }
            }

            // Если цветовая тема ещё не подключена
            mergedDicts.Add(themeDict);
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
                MaxRestoreButton_Click(sender, e);
            else
                DragMove();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MaxRestoreButton_Click(object sender, RoutedEventArgs e)
        {
            var area = SystemParameters.WorkArea;
            if (Width != area.Width || Height != area.Height || Left != area.Left || Top != area.Top)
            {
                Left = area.Left;
                Top = area.Top;
                Width = area.Width;
                Height = area.Height;
            }
            else
            {
                Width = 1280;
                Height = 800;
                Left = (SystemParameters.PrimaryScreenWidth - Width) / 2;
                Top = (SystemParameters.PrimaryScreenHeight - Height) / 2;
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
