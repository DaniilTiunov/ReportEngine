namespace ReportEngine.Shared.Helpers;

public static class FloatExtensions
{
    public static float Ceiling(this float value)
    {
        return MathF.Ceiling(value);
    }

    public static float? Ceiling(this float? value)
    {
        return value.HasValue ? MathF.Ceiling(value.Value) : null;
    }

    public static float Floor(this float value)
    {
        return MathF.Floor(value);
    }

    public static float? Floor(this float? value)
    {
        return value.HasValue ? MathF.Floor(value.Value) : null;
    }

    public static float Round(this float value, int decimals)
    {
        return MathF.Round(value, decimals);
    }

    public static float? Round(this float? value, int decimals)
    {
        return value.HasValue ? MathF.Round(value.Value, decimals) : null;
    }


    public static float RoundUp(this float value, int decimals)
    {
        var multiplier = MathF.Pow(10, decimals);
        return MathF.Ceiling(value * multiplier) / multiplier;
    }

    public static float? RoundUp(this float? value, int decimals)
    {
        if (!value.HasValue)
            return null;

        var multiplier = MathF.Pow(10, decimals);
        return MathF.Ceiling(value.Value * multiplier) / multiplier;
    }

    public static float RoundDown(this float value, int decimals)
    {
        var multiplier = MathF.Pow(10, decimals);
        return MathF.Floor(value * multiplier) / multiplier;
    }

    public static float? RoundDown(this float? value, int decimals)
    {
        if (!value.HasValue)
            return null;

        var multiplier = MathF.Pow(10, decimals);
        return MathF.Floor(value.Value * multiplier) / multiplier;
    }
}
