using Microsoft.WindowsAPICodePack.Shell.Interop;
using ReportEngine.App.Display;
using ReportEngine.App.Model.StandsModel;

namespace ReportEngine.App.AppHelpers;

public static class StandUniqNameHelper
{
    public static string SetUniqNameForStand(StandModel standModel, int offset = 1)
    {
        var standSerialNumber = standModel.SerialNumber; // например "25-02.222"

        if(string.IsNullOrEmpty(standSerialNumber))
            throw new Exception("Отсутствует серийный номер!");

        var parts = standSerialNumber.Split('.');

        if (parts.Length != 2)
            throw new Exception("Некорректный формат серийного номера");

        var baseName = parts[0]; // "25-02"

        if (!int.TryParse(parts[1], out var currentNumber))
            throw new Exception("Некорректный числовой суффикс");

        // Увеличиваем номер на offset
        var nextNumber = currentNumber + offset;

        return $"{baseName}.{nextNumber:D3}";
    }
}
