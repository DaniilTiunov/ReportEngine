using System.Globalization;

namespace ReportEngine.Extensions.Extensions;

public static class StringExtensions
{
    public static decimal ToDecimal(this string value)
    {
        return Convert.ToDecimal(value ?? string.Empty);
    }

    public static double ToDouble(this string value)
    {
        return Convert.ToDouble(value ?? string.Empty);
    }

    public static float ToFloat(this string value)
    {
        return float.Parse(value, CultureInfo.InvariantCulture.NumberFormat);
    }

    public static int ToInt(this string value)
    {
        return int.Parse(value ?? string.Empty);
    }

    public static DateOnly ToDateOnly(this string value)
    {
        return DateOnly.Parse(value ?? string.Empty);
    }
}