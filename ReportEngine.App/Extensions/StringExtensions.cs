namespace ReportEngine.App.Extensions;

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
        return Convert.ToSingle(value ?? string.Empty);
    }
}
