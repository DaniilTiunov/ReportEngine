using ReportEngine.Domain.Enums;
using System.Globalization;
using System.Windows.Data;

namespace ReportEngine.App.Display;

public class TranslateUserRole : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        switch (value)
        {
            case SystemRole.Admin:
                return "Администратор";
            case SystemRole.User:
                return "Пользователь";
            case SystemRole.Manager:
                return "Менеджер";
            default:
                return "Ошибка";
        }
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}