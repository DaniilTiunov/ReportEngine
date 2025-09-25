using System.Globalization;
using System.Windows;
using System.Windows.Data;
using ReportEngine.App.Services.Core;
using ReportEngine.Domain.Enums;

namespace ReportEngine.App.Display;

public class RoleToVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is SystemRole role && parameter is string requiredRoleString &&
            Enum.TryParse<SystemRole>(requiredRoleString, out var requiredRole))
        {
            return role == requiredRole ? Visibility.Visible : Visibility.Collapsed;
        }
        
        return Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
