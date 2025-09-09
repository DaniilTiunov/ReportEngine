using System.Globalization;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using ReportEngine.App.AppHelpers;
using Wpf.Ui.Controls;

namespace ReportEngine.App.Display;

public class ByteArrayImageConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        var bytes = value as byte[];
        
        if (bytes == null || bytes.Length == 0) 
            return null;

        try
        {
            var image = new BitmapImage();
            using (var ms = new MemoryStream(bytes))
            {
                ms.Position = 0;
                image.BeginInit();
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.StreamSource = ms;
                image.EndInit();
                image.Freeze();
            }
            return image;
        }
        catch
        {
            return null;
        }
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}