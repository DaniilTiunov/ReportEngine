using ReportEngine.Domain.Entities;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;

namespace ReportEngine.Export.Mapping;

public static class TemplateMapper
{
    public static Dictionary<string, string> GetPassportMapping(Stand stand)
    {
        var standInfo = stand;

        return new Dictionary<string, string>
        {
            { "{{stand_KKS_code}}", standInfo?.KKSCode ?? "N/A" },
            { "{{stand_Name}}", standInfo?.Design ?? "N/A" },
            { "{{stand_Manufacturer}}", "Изготовитель стенда?? Хз где брать" ?? "N/A" },
            { "{{stand_SerialNumber}}", standInfo?.SerialNumber ?? "N/A" },
            { "{{stand_YearManufacture}}", "Год изготовления стенда?? Хз где брать" ?? "N/A" },
            { "{{stand_Description}}", standInfo?.DesigneStand ?? "N/A" }
            // { "is_galvanized}}", }
        };
    }


    public static Dictionary<string, object> GetTechnologicalCardsMapping(Stand stand)
    {
        var standInfo = stand;

        return new Dictionary<string, object>
        {
            { "stand_KKS_code", standInfo?.KKSCode ?? "N/A" },
            { "stand_Name", standInfo?.Design ?? "N/A" },
            { "stand_Blueprint", ByteToImage(standInfo?.ImageData) },
            { "stand_Manufacturer", "Изготовитель стенда?? Хз где брать" ?? "N/A" },
            { "stand_SerialNumber", standInfo?.SerialNumber ?? "N/A" },
            { "stand_YearManufacture", "Год изготовления стенда?? Хз где брать" ?? "N/A" },
            { "stand_Description", standInfo?.DesigneStand ?? "N/A" }
            //{ "stand_Frame", standInfo?.StandFrames.First().Frame.Name ?? "N/A" },
            //{ "stand_FrameDesign", standInfo?.StandFrames?.First()?.Frame?.Designe ?? "N/A" }
        };
    }


    private static string ByteToImage(byte[] bytes)
    {
        try
        {
            if (bytes == null || bytes.Length == 0)
                return null;

            var tempPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".png");
            using (var ms = new MemoryStream(bytes))
            using (var img = Image.FromStream(ms))
            {
                img.Save(tempPath, ImageFormat.Png);
            }

            return tempPath;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Произошла ошибка: {ex.Message}", ConsoleColor.Red);
            return null;
        }
    }
}