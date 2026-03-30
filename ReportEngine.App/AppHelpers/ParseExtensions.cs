using System.Globalization;

namespace ReportEngine.App.AppHelpers;

public static class ParseExtensions
{
    public static float AsFloat(this string value, float defaultValue = 0f)
    {
        return float.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result)
            ? result
            : defaultValue;
    }

    public static double AsDouble(this string value, double defaultValue = 0d)
    {
        return double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result)
            ? result
            : defaultValue;
    }

    public static double AsDouble(this float value)
    {
        return Convert.ToDouble(value);
    }

    public static float AsFloat(this double value)
    {
        return Convert.ToSingle(value);
    }

    public static string AsString(this object value, string defaultValue = "")
    {
        return value?.ToString() ?? defaultValue;
    }
}
