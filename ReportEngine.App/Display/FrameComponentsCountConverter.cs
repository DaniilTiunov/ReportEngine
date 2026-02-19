using System.Globalization;
using System.Windows.Data;
using ReportEngine.App.ModelWrappers;

namespace ReportEngine.App.Display
{
    public class FrameComponentsCountConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var item = value as DisplayedComponent;

            if (item == null)
                return "";

            if (item.Count > 0)
                return item.Count.ToString();

            if (item.Length > 0)
                return $"{item.Length:0.##}";

            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}
