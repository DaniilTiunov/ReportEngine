using ReportEngine.Domain.Entities.BaseEntities.Interface;
using System.Globalization;
using System.Windows.Data;

namespace ReportEngine.App.Convert
{
    public class FrameDetailsToNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is IEnumerable<IBaseEquip> details && details.Any())
                return string.Join(", ", details.Select(d => d.Name));
            return "Нет деталей";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
