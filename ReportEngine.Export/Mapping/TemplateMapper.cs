using ReportEngine.Domain.Entities;

namespace ReportEngine.Export.Mapping;

public static class TemplateMapper
{
    public static Dictionary<string, object> GetPassportMapping(Stand stand)
    {
        var standInfo = stand;

        return new Dictionary<string, object>
        {
            { "stand_KKS_code", standInfo?.KKSCode ?? "N/A" },
            { "stand_Name", standInfo?.Design ?? "N/A" },
            { "stand_Manufacturer", "Изготовитель стенда?? Хз где брать" ?? "N/A" },
            { "stand_SerialNumber", standInfo?.SerialNumber ?? "N/A" },
            { "stand_YearManufacture", "Год изготовления стенда?? Хз где брать" ?? "N/A" },
            { "stand_Description", standInfo?.DesigneStand ?? "N/A" }
        };
    }

    public static Dictionary<string, object> GetTechnologicalCardsMapping(Stand stand)
    {
        var standInfo = stand;
        
        return new Dictionary<string, object>
        {
            { "stand_KKS_code", standInfo?.KKSCode ?? "N/A" },
            { "stand_Name", standInfo?.Design ?? "N/A" },
            { "stand_Manufacturer", "Изготовитель стенда?? Хз где брать" ?? "N/A" },
            { "stand_SerialNumber", standInfo?.SerialNumber ?? "N/A" },
            { "stand_YearManufacture", "Год изготовления стенда?? Хз где брать" ?? "N/A" },
            { "stand_Description", standInfo?.DesigneStand ?? "N/A" }
            //{ "stand_Frame", standInfo?.StandFrames.First().Frame.Name ?? "N/A" },
            //{ "stand_FrameDesign", standInfo?.StandFrames?.First()?.Frame?.Designe ?? "N/A" }
        };
    }
}