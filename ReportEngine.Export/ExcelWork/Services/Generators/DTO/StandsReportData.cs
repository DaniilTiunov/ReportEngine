namespace ReportEngine.Export.ExcelWork.Services.Generators.DTO;

public record StandsReportData(
    List<(string name, string unit, string quantity)> PipesList,
    List<(string name, string unit, string quantity)> ArmaturesList,
    List<(string name, string unit, string quantity)> TreeList,
    List<(string name, string unit, string quantity)> KmchList,
    List<(string name, string unit, string quantity)> DrainageParts,
    List<(string name, string unit, string quantity)> FramesList,
    List<(string name, string unit, string quantity)> SensorsHolders,
    List<(string name, string unit, string quantity)> ElectricalParts,
    List<(string name, string unit, string quantity)> OthersParts,
    List<(string name, string unit, string quantity)> Supplies
);