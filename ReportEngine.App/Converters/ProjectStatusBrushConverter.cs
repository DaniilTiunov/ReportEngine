using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using ReportEngine.Domain.Enums;

namespace ReportEngine.App.Converters;

public class ProjectStatusBrushConverter : IValueConverter
{
    private static readonly Dictionary<ProjectStatus, Brush> Brushes = new()
    {
        [ProjectStatus.ТКП] = new SolidColorBrush(Color.FromRgb(190, 190, 190)),
        [ProjectStatus.Расчёт] = new SolidColorBrush(Color.FromRgb(197, 228, 234)),
        [ProjectStatus.Завершен] = new SolidColorBrush(Color.FromRgb(199, 227, 208)),
        [ProjectStatus.Производство] = new SolidColorBrush(Color.FromRgb(69, 137, 156)),
        [ProjectStatus.Копия] = new SolidColorBrush(Color.FromRgb(216, 208, 227))
    };

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is ProjectStatus status &&
               Brushes.TryGetValue(status, out var brush)
            ? brush
            : System.Windows.Media.Brushes.Transparent;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
