namespace ReportEngine.Shared.Helpers;

public static class ComboBoxHelper
{
    public static TEnum ComboBoxChangedValue<TEnum>(string status) where TEnum : Enum
    {
        return (TEnum)Enum.Parse(typeof(TEnum), status);
    }
}