using System.Globalization;
using System.Windows;
using Mapster;
using ReportEngine.App.Model;
using ReportEngine.Import.Dto;

namespace ReportEngine.App.Mapping.Import;

public class ProjectInfoMapper : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        try
        {
            config.NewConfig<ProjectInfoDto, ProjectModel>()
                .Map(dest => dest.IsGalvanized,
                    src => ParseGalvanized(src.IsGalvanized))
                .Map(
                    dest => dest.HumanCost,
                    src => string.IsNullOrWhiteSpace(src.HumanCost)
                        ? (float?)null
                        : float.Parse(
                            src.HumanCost.Trim().Replace(',', '.'),
                            CultureInfo.InvariantCulture))
                .Map(dest => dest.Cost,
                    src => ParseCost(src.Cost))
                .Map(
                    dest => dest.CreationDate,
                    src => ParseDate(src.CreationDate))
                .Map(
                    dest => dest.OutOfProduction,
                    src => ParseDate(src.OutOfProduction))
                .Map(
                    dest => dest.EndDate,
                    src => ParseDate(src.EndDate))
                .Map(
                    dest => dest.StartDate,
                    src => ParseDate(src.StartDate));
        }
        catch (Exception e)
        {
            MessageBox.Show($"Ошибка в конвертации {e.Message}, {e.StackTrace}");
            throw;
        }
    }

    private static bool ParseGalvanized(string? value)
    {
        return string.Equals(
            value?.Trim(),
            "да",
            StringComparison.OrdinalIgnoreCase);
    }
    
    private static decimal ParseCost(string? value)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(value))
                return 0m;

            var normalized = value.Trim()
                .Replace(" ", "")
                .Replace("\u00A0", "")
                .Replace("\u202F", "")
                .Replace(',', '.');

            if (decimal.TryParse(
                    normalized,
                    NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
                    CultureInfo.InvariantCulture,
                    out var cost))
                return cost;
        
            return 0m;
        }
        catch (Exception e)
        {
            MessageBox.Show($"Ошибка в конвертации стоимости {e.Message}, {e.StackTrace}");
            throw;
        }
    }

    private static DateTime? ParseDate(string? value)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;

            string[] formats =
            [
                "dd.MM.yyyy",
                "d.M.yyyy",
                "yyyy-MM-dd"
            ];

            return DateTime.TryParseExact(
                value.Trim(),
                formats,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var date)
                ? date
                : null;
        }
        catch (Exception e)
        {
            MessageBox.Show($"Ошибка в конвертации времени {e.Message}, {e.StackTrace}");
            throw;
        }
    }
}