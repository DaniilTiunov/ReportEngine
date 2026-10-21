using ClosedXML.Excel;
using ReportEngine.Import.Abstractions;
using ReportEngine.Import.Configuration;
using ReportEngine.Import.Dto;

namespace ReportEngine.Import.Services.Project;

public class ProjectInfoParser : IProjectInfoParser
{
    private readonly ProjectInfoCellConfig _projectInfoCellConfig = new()
    {
        CellNumber = "C3",
        CellDescription = "C4",
        CellCreationDate = "G5",
        CellCompany = "B1",
        CellObject = "B2",
        CellStandCount = "C9",
        CellCost = "C10",
        CellStatus = "G9",
        CellStartDate = "G3",
        CellOutOfProduction = "G4",
        CellEndDate = "G6",
        CellOrderCustomer = "C5",
        CellRequestProduction = "C6",
        CellMarkPlus = "C17",
        CellMarkMinus = "C18",
        CellIsGalvanized = "C14",
        CellHumanCost = "C14",
        CellManager = "G10"
    };


    public ProjectInfoDto Parse(IXLWorksheet worksheet)
    {
        var projectInfo = new ProjectInfoDto
        {
            Number = GetCellValue(worksheet, _projectInfoCellConfig.CellNumber ?? "A1"),
            Description = GetCellValue(worksheet, _projectInfoCellConfig.CellDescription ?? "A1"),
            CreationDate = GetCellValue(worksheet, _projectInfoCellConfig.CellCreationDate ?? "A1"),
            Company = GetCellValue(worksheet, _projectInfoCellConfig.CellCompany ?? "A1"),
            Object = GetCellValue(worksheet, _projectInfoCellConfig.CellObject ?? "A1"),
            StandCount = GetCellValue(worksheet, _projectInfoCellConfig.CellStandCount ?? "A1"),
            Cost = GetCellValue(worksheet, _projectInfoCellConfig.CellCost ?? "A1"),
            Status = GetCellValue(worksheet, _projectInfoCellConfig.CellStatus ?? "A1"),
            StartDate = GetCellValue(worksheet, _projectInfoCellConfig.CellStartDate ?? "A1"),
            OutOfProduction = GetCellValue(worksheet, _projectInfoCellConfig.CellOutOfProduction ?? "A1"),
            EndDate = GetCellValue(worksheet, _projectInfoCellConfig.CellEndDate ?? "A1"),
            OrderCustomer = GetCellValue(worksheet, _projectInfoCellConfig.CellOrderCustomer ?? "A1"),
            RequestProduction = GetCellValue(worksheet, _projectInfoCellConfig.CellRequestProduction ?? "A1"),
            MarkPlus = GetCellValue(worksheet, _projectInfoCellConfig.CellMarkPlus ?? "A1"),
            MarkMinus = GetCellValue(worksheet, _projectInfoCellConfig.CellMarkMinus ?? "A1"),
            IsGalvanized = GetCellValue(worksheet, _projectInfoCellConfig.CellIsGalvanized ?? "A1"),
            HumanCost = GetCellValue(worksheet, _projectInfoCellConfig.CellHumanCost ?? "A1"),
            Manager = GetCellValue(worksheet, _projectInfoCellConfig.CellManager ?? "A1")
        };

        return projectInfo;
    }

    private string GetCellValue(
        IXLWorksheet worksheet,
        string cell)
    {
        return worksheet
            .Cell(cell)
            .GetString()
            .Trim();
    }
}