using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using MaterialDesignThemes.Wpf;
using ReportEngine.App.Services.Notification;
using ReportEngine.App.ViewModels;
using ReportEngine.App.ViewModels.TreeView;
using ReportEngine.Shared.Config.DebugConsol;

namespace ReportEngine.App.Views.Controls;

public partial class TreeProjectView : UserControl, IDisposable
{
    private readonly ExceptionService _exceptionService;
    private readonly ProjectViewModel _projectViewModel;
    private readonly ContainersViewModel _containersViewModel;
    private bool _disposed;

    public TreeProjectView(
        TreeViewModel treeViewModel,
        ProjectViewModel projectViewModel,
        ExceptionService exceptionService, 
        ContainersViewModel containersViewModel)
    {
        InitializeComponent();
        _projectViewModel = projectViewModel;
        _exceptionService = exceptionService;
        _containersViewModel = containersViewModel;
        DataContext = treeViewModel;
    }

    public void Dispose()
    {
        if (_disposed) return;

        DataContext = null;

        if (Resources != null)
            Resources.Clear();

        _disposed = true;
    }
    
    private string GetHeaderText(TreeViewItem item)
    {
        if (item.Header is StackPanel stackPanel)
        {
            // Ищем TextBlock в StackPanel
            foreach (var child in stackPanel.Children)
            {
                if (child is TextBlock textBlock)
                {
                    return textBlock.Text;
                }
            }
        }
        // Если Header - простая строка (для элементов без иконок)
        return item.Header?.ToString() ?? string.Empty;
    }

    private PackIconKind GetIconKind(TreeViewItem item)
    {
        if (item.Header is StackPanel stackPanel)
        {
            foreach (var child in stackPanel.Children)
            {
                if (child is PackIcon icon)
                {
                    return icon.Kind;
                }
            }
        }
        
        return PackIconKind.Folder;
    }

    private void OpenCurrentView(object sender, MouseButtonEventArgs e)
    {
        _exceptionService.SafeExecute(() =>
        {
            var treeViewItem = NavigationTree.SelectedItem as TreeViewItem;
            if (treeViewItem?.Tag != null)
            {
                var icon = GetIconKind(treeViewItem);
                var header = GetHeaderText(treeViewItem);
                var tag = treeViewItem.Tag.ToString();
                LoadTreeContent(tag, header, icon);
            }
        });
    }

    private void CloseCurrentView(object sender, RoutedEventArgs e)
    {
        _exceptionService.SafeExecute(() =>
        {
            // Сначала пытаемся получить связанную вкладку из Tag кнопки
            if (sender is Button btn && btn.Tag is TabItem taggedTab)
            {
                if (MainTabControl.Items.Contains(taggedTab))
                    MainTabControl.Items.Remove(taggedTab);

                return;
            }

            // Фолбэк: если тег не установлен, удаляем текущую выбранную вкладку
            if (MainTabControl.SelectedItem is TabItem selectedTab)
                MainTabControl.Items.Remove(selectedTab);
        });
    }

    private void LoadTreeContent(string tag, string header, PackIconKind iconKind)
    {
        _exceptionService.SafeExecute(() =>
        {
            if (string.IsNullOrEmpty(tag))
                return;

            var content = CreateCurrentContent(tag);
            if (content == null)
                return; // Не добавлять вкладку, если контент не создан

            if (CheckForOpenedTabs(content, tag))
                return; // Вкладка уже открыта, переключаемся на неё

            var tabItem = new TabItem
            {
                Tag = tag,
                Content = content,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                VerticalContentAlignment = VerticalAlignment.Stretch, 
                Style = (Style)FindResource(typeof(TabItem))
            };

            tabItem.Header = CreateTabItemHeader(header, tabItem, iconKind);

            MainTabControl.Items.Add(tabItem);
            MainTabControl.SelectedItem = tabItem;
        });
    }

    private UserControl CreateCurrentContent(string tag)
    {
        try
        {
            if (string.IsNullOrEmpty(tag))
                return null;

            return tag switch
            {
                "ProjectCard" => ApplyAnimation(new ProjectCardView(_projectViewModel)),
                "ProjectPreview" => ApplyAnimation(new ProjectPreview(_projectViewModel)),
                "StandsContainer" => ApplyAnimation(new StandsContainerView(_containersViewModel)),
                "DockViewer" => ApplyAnimation(new DockViewerView(new DockViewerViewModel()))
            };
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
            DebugConsole.WriteLine(ex, ConsoleColor.Red);
            return null;
        }
    }

    private UIElement CreateTabItemHeader(
        string headerName, 
        TabItem parentTab,
        PackIconKind iconKind)
    {
        var header = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Margin = new Thickness(0, 0, 0, 0)
        };

        var icon = new PackIcon
        {
            Kind = iconKind,
            Width = 16,
            Height = 16,
            Margin = new Thickness(0, 0, 8, 0),
            Foreground = (Brush)Application.Current.FindResource("PrimaryForeground")
        };

        var headerText = new TextBlock
        {
            Text = headerName,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(0, 0, 5, 0),
            FontSize = 16,
            FontFamily = new FontFamily("Bahnschrift"),
            Style = (Style)FindResource(typeof(TextBlock))
        };

        var closeButton = new Button
        {
            Content = "✕",
            Width = 16,
            Height = 16,
            Padding = new Thickness(0),
            Margin = new Thickness(0),
            VerticalAlignment = VerticalAlignment.Center,
            Style = (Style)FindResource(typeof(Button))
        };
        

        closeButton.Tag = parentTab;
        closeButton.Click += CloseCurrentView;

        header.Children.Add(icon);
        header.Children.Add(headerText);
        header.Children.Add(closeButton);
        return header;
    }

    private bool CheckForOpenedTabs(UserControl control, string tag)
    {
        if (control == null)
            return false;

        foreach (var item in MainTabControl.Items.OfType<TabItem>())
            if (item.Tag is string existingTag && !string.IsNullOrEmpty(existingTag))
            {
                if (string.Equals(existingTag, tag, StringComparison.Ordinal))
                {
                    MainTabControl.SelectedItem = item;
                    return true;
                }
            }
            else
            {
                if (item.Content != null && item.Content.GetType() == control.GetType())
                {
                    MainTabControl.SelectedItem = item;
                    return true;
                }
            }

        return false;
    }

    private UserControl ApplyAnimation(UserControl control)
    {
        control.Opacity = 0;
        control.RenderTransform = new TranslateTransform(0, 20);

        control.Dispatcher.BeginInvoke(new Action(() =>
        {
            var storyboard = new Storyboard();

            var fadeAnimation = new DoubleAnimation
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(250),
                EasingFunction = new QuadraticEase
                {
                    EasingMode = EasingMode.EaseOut
                }
            };

            Storyboard.SetTarget(fadeAnimation, control);
            Storyboard.SetTargetProperty(fadeAnimation, new PropertyPath("Opacity"));

            var slideAnimation = new DoubleAnimation
            {
                From = 20,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(250),
                EasingFunction = new QuadraticEase
                {
                    EasingMode = EasingMode.EaseOut
                }
            };

            Storyboard.SetTarget(slideAnimation, control);
            Storyboard.SetTargetProperty(slideAnimation,
                new PropertyPath("(UIElement.RenderTransform).(TranslateTransform.Y)"));

            storyboard.Children.Add(fadeAnimation);
            storyboard.Children.Add(slideAnimation);

            storyboard.Begin();
        }), DispatcherPriority.Loaded);

        return control;
    }

    ~TreeProjectView()
    {
        Dispose();
    }
}
