using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using ReportEngine.App.AppHelpers;
using ReportEngine.App.ViewModels;
using ReportEngine.Shared.Config.DebugConsol;

namespace ReportEngine.App.Views.Controls;

public partial class TreeProjectView : UserControl, IDisposable
{
    private readonly ProjectViewModel _projectViewModel;
    private readonly Dictionary<string, Action> _tagActionMap;
    private readonly Dictionary<string, Action> _tagCalculateMap;
    private bool _disposed;

    public TreeProjectView(ProjectViewModel projectViewModel)
    {
        InitializeComponent();
        _projectViewModel = projectViewModel;
        DataContext = projectViewModel;

        _tagActionMap = new Dictionary<string, Action>
        {
            { "SummaryReport", () => _projectViewModel.OnCreateSummaryReportCommandExecuted(null) },
            { "ComponentsList", () => _projectViewModel.OnComponentsListReportCommandExecuted(null) },
            { "NamePlates", () => _projectViewModel.OnCreateNameplatesReportCommandExecuted(null) },
            { "MarksReport", () => _projectViewModel.OnCreateMarksReportCommandExecuted(null) },
            { "ProductionList", () => _projectViewModel.OnCreateProductionReportCommandExecuted(null) },
            { "FinPlan", () => _projectViewModel.OnCreateFinplanReportCommandExecuted(null) },
            { "ContainersReport", () => _projectViewModel.OnCreateContainerReportCommandExecuted(null) },
            { "Passport", () => _projectViewModel.OnCreatePassportReportCommandExecuted(null) },
            { "TechCards", () => _projectViewModel.OnCreateTechnologicalCardsCommandExecute(null) }
        };
        _tagCalculateMap = new Dictionary<string, Action>
        {
            { "CalculateProject", () => _projectViewModel.OnCalculateProjectCommandExecuted(null) },
            { "UpdateAllProps", () => _projectViewModel.OnUpdateStandsAfterEquipsCommandExecuted(null) }
        };
    }

    public void Dispose()
    {
        if (_disposed) return;

        DataContext = null;

        if (Resources != null)
            Resources.Clear();

        _disposed = true;
    }

    private void OpenCurrentView(object sender, MouseButtonEventArgs e)
    {
        ExceptionHelper.SafeExecute(() =>
        {
            var treeViewItem = MainTree.SelectedItem as TreeViewItem;
            if (treeViewItem?.Tag != null)
            {
                var header = treeViewItem.Header.ToString();
                var tag = treeViewItem.Tag.ToString();
                LoadTreeContent(tag, header);
            }
        });
    }

    private void CloseCurrentView(object sender, RoutedEventArgs e)
    {
        ExceptionHelper.SafeExecute(() =>
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

    private void LoadTreeContent(string tag, string header)
    {
        ExceptionHelper.SafeExecute(() =>
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
                Content = content
            };

            tabItem.Header = CreaterTabItemHeader(header, tabItem);

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
                //"Stand" => ApplyAnimation(new StandView(_projectViewModel)),
                //"StandObv" => ApplyAnimation(new StandObvView(_projectViewModel)),
                //"FrameDrainages" => ApplyAnimation(new FrameDrainagesView(_projectViewModel)),
                "ProjectPreview" => ApplyAnimation(new ProjectPreview(_projectViewModel)),
                "StandsContainer" => ApplyAnimation(new StandsContainerView(_projectViewModel)),
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

    private UIElement CreaterTabItemHeader(string headerName, TabItem parentTab)
    {
        var header = new StackPanel
        {
            Orientation = Orientation.Horizontal,
            Margin = new Thickness(0, 0, 0, 0)
        };

        var headerText = new TextBlock
        {
            Text = headerName,
            VerticalAlignment = VerticalAlignment.Center,
            Margin = new Thickness(0, 0, 5, 0),
            FontSize = 16,
            FontFamily = new FontFamily("Bahnschrift")
        };

        var closeButton = new Button
        {
            Content = "✕",
            Width = 16,
            Height = 16,
            Padding = new Thickness(0),
            Margin = new Thickness(0),
            VerticalAlignment = VerticalAlignment.Center
        };

        closeButton.Tag = parentTab;
        closeButton.Click += CloseCurrentView;

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

    private async void ReportTree_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (ReportTree.SelectedItem is TreeViewItem treeViewItem && treeViewItem.Tag is string tag)
            if (_tagActionMap.TryGetValue(tag, out var action))
                action();
    }

    private async void CalculateTree_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (CalculateTree.SelectedItem is TreeViewItem treeViewItem && treeViewItem.Tag is string tag)
            if (_tagCalculateMap.TryGetValue(tag, out var action))
                action();
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
