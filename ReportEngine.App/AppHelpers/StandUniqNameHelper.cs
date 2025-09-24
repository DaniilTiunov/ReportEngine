using System.Text.RegularExpressions;
using ReportEngine.App.Model.StandsModel;

namespace ReportEngine.App.AppHelpers;

public static class StandUniqNameHelper
{
    public static string SetUniqNameForStand(StandModel standModel)
    {
        var standSerialNumber = standModel.SerialNumber; // например "Эп.25-02.222"

        // Выделяем префикс (всё до первой цифры)
        int firstDigitIndex = standSerialNumber.IndexOfAny("0123456789".ToCharArray());
        string prefix = firstDigitIndex > 0 ? standSerialNumber.Substring(0, firstDigitIndex) : "";
        string baseAndNumber = standSerialNumber.Substring(firstDigitIndex); // "25-02.222"

        // Разделяем на базовую часть и числовой суффикс
        var parts = baseAndNumber.Split('.');
        if (parts.Length != 2)
            throw new Exception("Некорректный формат серийного номера");

        string baseName = parts[0]; // "25-02"
        if (!int.TryParse(parts[1], out int number))
            throw new Exception("Некорректный числовой суффикс");

        // Увеличиваем номер
        number++;

        // Формируем новый серийный номер
        return $"{prefix}{baseName}.{number}";
    }
}