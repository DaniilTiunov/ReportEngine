using ClosedXML.Excel;
using ReportEngine.Domain.Entities;
using ReportEngine.Import.Configuration;
using ReportEngine.Import.Dto;
using ReportEngine.Import.Services.Excel;

namespace ReportEngine.Import.Services.Stand;

public class ContainerParser
{
    private readonly ContainerCellConfig _containerCellConfig = new();

    private readonly ExcelAddressingService _excelAddressingService;

    public ContainerParser(ExcelAddressingService excelAddressingService)
    {
        _excelAddressingService = excelAddressingService;
    }


    public List<ContainerDto> Parse(IXLWorksheet worksheet)
    {
        var results = new List<ContainerDto>();

        var startDataRow = 2;

        var (_, lastRow) = _excelAddressingService.FindSectionRows(worksheet, "Код KKS");

        for (var currrentRow = startDataRow; currrentRow <= lastRow; currrentRow++)
        {
            //парсим данные стенда
            var parserdStand = ParseStand(worksheet, currrentRow);

            //парсим значение номера места
            var containerPlaceNumberCell = worksheet.Cell(currrentRow, _containerCellConfig.ContainerPlaceNumberColumn);
            var containerPlaceNumberValue = GetMergedCellValue(containerPlaceNumberCell);

            //проверяем, заведен ли контейнер в результаты
            var existingContainer = results
                .FirstOrDefault(container => container.PlaceNumber == containerPlaceNumberValue);


            //если такого нет - создаем и добавляем его
            if (existingContainer is null)
            {
                var containerWeightCell = worksheet.Cell(currrentRow, _containerCellConfig.ContainerWeightColumn);
                var containerNameCell = worksheet.Cell(currrentRow, _containerCellConfig.ContainerNameColumn);

                var containerWeightValue = GetMergedCellValue(containerWeightCell);
                var containerNameValue = GetMergedCellValue(containerNameCell);


                var newContainer = new ContainerDto()
                {
                    PlaceNumber = containerPlaceNumberValue,
                    ContainerWeight = containerWeightValue,
                    ContainerName = containerNameValue,
                    Stands = new()
                };

                results.Add(newContainer);

                existingContainer = newContainer;
            }


            existingContainer.Stands.Add(parserdStand);

        }

        return results;
    }



    private StandInContainerDto ParseStand(IXLWorksheet worksheet, int row)
    {
        var stand = new StandInContainerDto()
        {
            PlaceNumberInContainer = worksheet.Cell(row, _containerCellConfig.StandPlaceNumberInContainerColumn).GetString(),
            DeviceName = worksheet.Cell(row, _containerCellConfig.StandDeviceNameInContainerColumn).GetString(),
            SerialNumber = worksheet.Cell(row, _containerCellConfig.StandSerialNameInContainerColumn).GetString(),
            KKS = worksheet.Cell(row, _containerCellConfig.StandKKSInContainerColumn).GetString(),
            Count = worksheet.Cell(row, _containerCellConfig.StandCountInContainerColumn).GetString(),
        };


        var frameWidthCell = worksheet.Cell(row, _containerCellConfig.StandFrameWidthInContainerColumn);
        stand.FrameWidth = GetMergedCellValue(frameWidthCell);

        return stand;
    }



    private string GetMergedCellValue(IXLCell cell)
    {
        return cell.IsMerged()
               ? cell.MergedRange().FirstCell().GetString()
               : cell.GetString();
    }

}