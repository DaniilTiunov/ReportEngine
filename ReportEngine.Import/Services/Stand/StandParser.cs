using ClosedXML.Excel;
using ReportEngine.Extensions.Extensions.ClosedXmlExtensions;
using ReportEngine.Import.Abstractions;
using ReportEngine.Import.Configuration;
using ReportEngine.Import.Dto;

namespace ReportEngine.Import.Services.Stand;

public class StandParser : IStandParser
{
    private readonly ComponentParser _componentParser;

    private readonly StandCellConfig _standCellConfig = new()
    {
        CellNumber = "C3",
        CellKKSCode = "B1",
        CellDesign = "B2",
        CellDevices = "C4",
        CellBraceType = "C5",
        CellWidth = "C6",
        CellSerialNumber = "F2",
        CellWeight = "G4",
        CellStandSummCost = "G3",
        CellDesigneStand = "G5",
        CellComments = "G5"
    };

    public StandParser(ComponentParser componentParser)
    {
        _componentParser = componentParser;
    }
    
    public List<StandDto> Parse(IXLWorksheets worksheets)
    {
        var stands = new List<StandDto>();
        
        foreach (var worksheet in worksheets.Skip(1))
        {
            var stand =  ParseStand(worksheet);
            stands.Add(stand);
        }
        
        return stands;
    }

    private StandDto ParseStand(IXLWorksheet worksheet)
    {
        return new StandDto
        {
            Number = worksheet.GetCellValue(_standCellConfig.CellNumber),
            KKSCode = worksheet.GetCellValue(_standCellConfig.CellKKSCode),
            Design = worksheet.GetCellValue(_standCellConfig.CellDesign),
            Devices = worksheet.GetCellValue(_standCellConfig.CellDevices),
            BraceType = worksheet.GetCellValue(_standCellConfig.CellBraceType),
            Width = worksheet.GetCellValue(_standCellConfig.CellWidth),
            SerialNumber = worksheet.GetCellValue(_standCellConfig.CellSerialNumber),
            Weight = worksheet.GetCellValue(_standCellConfig.CellWeight),
            StandSummCost = worksheet.GetCellValue(_standCellConfig.CellStandSummCost),
            DesigneStand = worksheet.GetCellValue(_standCellConfig.CellDesigneStand),
            Comments = worksheet.GetCellValue(_standCellConfig.CellComments),
            AdditionalPurposes = _componentParser.Parse(worksheet, "ДОПОЛНИТЕЛЬНЫЕ КОМПОНЕНТЫ"),
            DrainagePurposes = _componentParser.Parse(worksheet, "ДРЕНАЖ"),
            ElectricalPurposes = _componentParser.Parse(worksheet, "ЭЛЕКТРИКА")
        };
    }
}