using ReportEngine.App.Model.StandsModel;

namespace ReportEngine.App.AppHelpers;

public static class StandUniqNameHelper
{
    public static string SetUniqNameForStand(StandModel standModel)
    {
        var standSerialNumber = standModel.SerialNumber; // например "25-02.222"

        var parts = standSerialNumber.Split('.');
        if (parts.Length != 2)
            throw new Exception("Некорректный формат серийного номера");

        var baseName = parts[0]; // "25-02"

        if (!int.TryParse(parts[1], out var currentNumber))
            throw new Exception("Некорректный числовой суффикс");

        // Увеличиваем номер
        var nextNumber = currentNumber + 1;

        return $"{baseName}.{nextNumber:D3}";
    }
}