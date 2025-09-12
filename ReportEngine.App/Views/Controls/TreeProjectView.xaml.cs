using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ReportEngine.App.AppHelpers;
using ReportEngine.App.ViewModels;
using ReportEngine.Shared.Config.DebugConsol;

namespace ReportEngine.App.Views.Controls;

public partial class TreeProjectView : UserControl, IDisposable
{
    private readonly ProjectViewModel _projectViewModel;
    private bool _disposed;

    public TreeProjectView(ProjectViewModel projectViewModel)
    {
        InitializeComponent();
        _projectViewModel = projectViewModel;
        DataContext = projectViewModel;
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
            var tabItem = MainTabControl.SelectedItem as TabItem;
            if (tabItem != null)
            {
                MainTabControl.Items.Remove(tabItem);
            }
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

            if(CheckForOpenedTabs(content, tag))
                return; // Вкладка уже открыта, переключаемся на неё

            var tabControl = new TabItem
            {
                Header = CreaterTabItemHeader(header),
                Content = content,
                FontSize = 16
            };

            MainTabControl.Items.Add(tabControl);
            MainTabControl.SelectedItem = tabControl;
        });
    }

    private UserControl CreateCurrentContent(string tag)
    {
        try
        {
            if (string.IsNullOrEmpty(tag))
                return null;
           
            UserControl control = tag switch
            {
                "ProjectCard" => new ProjectCardView(_projectViewModel),
                "Stand" => new StandView(_projectViewModel),
                "StandObv" => new StandObvView(_projectViewModel),
                "FrameDrainages" => new FrameDrainagesView(_projectViewModel),
                "ProjectPreview" => new ProjectPreview(_projectViewModel)
            };
            
            return control;
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
            DebugConsole.WriteLine(ex, ConsoleColor.Red);
            return null;
        }
    }

    private UIElement CreaterTabItemHeader(string headerName)
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
            Margin = new Thickness(0, 0, 5, 0)
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
        {
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
        }
        
        return false;
    }

    public void Dispose()
    {
        if (_disposed) return;

        DataContext = null;

        if (Resources != null)
            Resources.Clear();

        _disposed = true;
    }

    ~TreeProjectView()
    {
        Dispose();
    }
}