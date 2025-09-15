using ReportEngine.Shared.Config.Directory;
using System.Globalization;
using System.Windows.Data;

namespace ReportEngine.App.Display;

public class ObvyazkaImagePathConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is string imageName && !string.IsNullOrEmpty(imageName))
            return DirectoryHelper.GetImagesPath(imageName);
        return null;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}