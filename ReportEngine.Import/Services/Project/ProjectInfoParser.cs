using ClosedXML.Excel;
using ReportEngine.Extensions.Extensions.ClosedXmlExtensions;
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
        CellHumanCost = "C11",
        CellManager = "G10"
    };
    
    public ProjectInfoDto Parse(IXLWorksheet worksheet)
    {
        var projectInfo = new ProjectInfoDto
        {
            Number = worksheet.GetCellValue(_projectInfoCellConfig.CellNumber),
            Description = worksheet.GetCellValue(_projectInfoCellConfig.CellDescription),
            CreationDate = worksheet.GetCellValue(_projectInfoCellConfig.CellCreationDate),
            Company = worksheet.GetCellValue(_projectInfoCellConfig.CellCompany),
            Object = worksheet.GetCellValue(_projectInfoCellConfig.CellObject),
            StandCount = worksheet.GetCellValue(_projectInfoCellConfig.CellStandCount),
            Cost = worksheet.GetCellValue(_projectInfoCellConfig.CellCost),
            Status = worksheet.GetCellValue(_projectInfoCellConfig.CellStatus),
            StartDate = worksheet.GetCellValue(_projectInfoCellConfig.CellStartDate),
            OutOfProduction = worksheet.GetCellValue(_projectInfoCellConfig.CellOutOfProduction),
            EndDate = worksheet.GetCellValue(_projectInfoCellConfig.CellEndDate),
            OrderCustomer = worksheet.GetCellValue(_projectInfoCellConfig.CellOrderCustomer),
            RequestProduction = worksheet.GetCellValue(_projectInfoCellConfig.CellRequestProduction),
            MarkPlus = worksheet.GetCellValue(_projectInfoCellConfig.CellMarkPlus),
            MarkMinus = worksheet.GetCellValue(_projectInfoCellConfig.CellMarkMinus),
            IsGalvanized = worksheet.GetCellValue(_projectInfoCellConfig.CellIsGalvanized),
            HumanCost = worksheet.GetCellValue(_projectInfoCellConfig.CellHumanCost),
            Manager = worksheet.GetCellValue(_projectInfoCellConfig.CellManager)
        };

        return projectInfo;
    }
}