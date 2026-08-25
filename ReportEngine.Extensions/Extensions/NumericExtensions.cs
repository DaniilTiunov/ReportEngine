namespace ReportEngine.Extensions.Extensions;

public static class NumericExtensions
{
    public static float ToFloat(this double value)
    {
        return (float)value;
    }

    public static double ToDouble(this float value)
    {
        return value;
    }
}