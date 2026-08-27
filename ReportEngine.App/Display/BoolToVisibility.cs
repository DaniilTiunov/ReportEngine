using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ReportEngine.App.Display;

public class BoolToVisibility : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool b)
        {
            if(b)
                return Visibility.Visible;
        }
        
        return Visibility.Collapsed;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}