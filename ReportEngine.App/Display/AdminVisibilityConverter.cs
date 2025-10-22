using System.Globalization;
using System.Windows;
using System.Windows.Data;
using ReportEngine.Domain.Enums;

namespace ReportEngine.App.Display;

public class AdminVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is SystemRole role && role == SystemRole.Admin)
            return Visibility.Visible;

        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}