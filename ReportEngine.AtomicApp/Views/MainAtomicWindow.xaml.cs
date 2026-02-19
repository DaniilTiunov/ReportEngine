using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace ReportEngine.AtomicApp.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            StandardTheme(null, null);

            MainWindow_StartUpState();

            InitializeComponent();


            StateChanged += MainWindow_StateChanges;
        }

        private void ShowCalculator(object sender, RoutedEventArgs e)
        {
            Process.Start("calc.exe");
        }

        private void ShowNotepad(object sender, RoutedEventArgs e)
        {
            Process.Start("notepad.exe");
        }

        private void ChangeDarkTheme(object sender, RoutedEventArgs e)
        {
            ChangesTheme("/Resources/Dictionaries/ColorThemes/DarkTheme.xaml");
        }

        private void StandardTheme(object sender, RoutedEventArgs e)
        {
            ChangesTheme("/Resources/Dictionaries/ColorThemes/LightTheme.xaml");
        }

        private void MangoParadiseTheme(object sender, RoutedEventArgs e)
        {
            ChangesTheme("/Resources/Dictionaries/ColorThemes/MangoParadiseTheme.xaml");
        }

        private void BubbleGumTheme(object sender, RoutedEventArgs e)
        {
            ChangesTheme("/Resources/Dictionaries/ColorThemes/BubbleGumTheme.xaml");
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

            mergedDicts.Add(themeDict);
        }

        private void MainWindow_StateChanges(object? sender, EventArgs e)
        {
            if (WindowState == WindowState.Maximized)
                WindowState = WindowState.Normal;
        }

        private void MainWindow_StartUpState()
        {
            var area = SystemParameters.WorkArea;
            Left = area.Left;
            Top = area.Top;
            Width = area.Width;
            Height = area.Height;
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
