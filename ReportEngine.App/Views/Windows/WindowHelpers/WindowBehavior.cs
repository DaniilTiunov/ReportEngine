using System.Windows;
using System.Windows.Input;

namespace ReportEngine.App.Views.Windows.WindowHelpers;

public static class WindowBehavior
{
    public static readonly DependencyProperty CloseOnEscapeProperty =
        DependencyProperty.RegisterAttached(
            "CloseOnEscape",
            typeof(bool),
            typeof(WindowBehavior),
            new PropertyMetadata(false, OnCloseOnEscapeChanged));

    public static void SetCloseOnEscape(DependencyObject element, bool value)
    {
        element.SetValue(CloseOnEscapeProperty, value);
    }

    public static bool GetCloseOnEscape(DependencyObject element)
    {
        return (bool)element.GetValue(CloseOnEscapeProperty);
    }

    private static void OnCloseOnEscapeChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        if (d is not Window window)
            return;

        if ((bool)e.NewValue)
            window.PreviewKeyDown += Window_PreviewKeyDown;
        else
            window.PreviewKeyDown -= Window_PreviewKeyDown;
    }

    private static void Window_PreviewKeyDown(
        object sender,
        KeyEventArgs e)
    {
        if (e.Key != Key.Escape)
            return;

        if (sender is Window window)
        {
            window.Close();
            e.Handled = true;
        }
    }
}
