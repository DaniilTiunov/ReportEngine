using ClosedXML.Excel;
using ReportEngine.Import.Configuration;
using ReportEngine.Import.Dto;
using ReportEngine.Import.Services.Excel;
using System.Globalization;

namespace ReportEngine.Import.Services.Stand;

public class ObvParser
{
    private readonly string _categoryName = "ОБВЯЗКИ СТЕНДА";

    private readonly ComponentParser _componentParser;

    private readonly ExcelAddressingService _excelAddressingService;
    private readonly ObvCellConfig _obvCellConfig = new();


    public ObvParser(ExcelAddressingService excelAddressingService, ComponentParser componentParser)
    {
        _excelAddressingService = excelAddressingService;
        _componentParser = componentParser;
    }

    public List<ObvyazkaDto> Parse(IXLWorksheet worksheet)
    {
        var results = new List<ObvyazkaDto>();

        var allObvTitleRow = _excelAddressingService.FindSectionStartRow(worksheet, _categoryName);

        //начинаем с первой
        var currentObvStartRow = allObvTitleRow + 1;

        while (HasObvDataAt(worksheet, currentObvStartRow + 1))
        {
            var (obv, lastObvRow) = ParseObv(worksheet, currentObvStartRow);

            if (!obv.IsEmpty())
                results.Add(obv);

            currentObvStartRow = lastObvRow + 2;
        }

        return results;
    }

    private (ObvyazkaDto obv, int lastObvRow) ParseObv(IXLWorksheet worksheet, int obvStartRow)
    {
        var result = new ObvyazkaDto();

        //вытаскиваем номер и тип обвязки
        var commonObvInfoRow = obvStartRow + 1;

        result.Number = worksheet.Cell(commonObvInfoRow, _obvCellConfig.ObvNumberColumn).Value.ToString();
        result.ObvType = worksheet.Cell(commonObvInfoRow, _obvCellConfig.ObvTypeColumn).Value.ToString();

        // ID находится в строке заголовка обвязки, на строку выше номера и типа.
        var idCell = worksheet.Cell(obvStartRow, _obvCellConfig.ObvyazkaIdColumn);
        if (!int.TryParse(idCell.GetString().Trim(), NumberStyles.Integer,
                CultureInfo.InvariantCulture, out var obvyazkaId) || obvyazkaId <= 0)
        {
            throw new FormatException(
                $"На листе '{worksheet.Name}' в ячейке {idCell.Address} " +
                $"указан некорректный ID обвязки: '{idCell.GetString()}'. " +
                "Ожидается положительное целое число из справочника обвязок.");
        }

        result.ObvyazkaId = obvyazkaId;

        //находим секцию состав целиком
        var (structureStartRow, structureEndRow) = (obvStartRow + 2, obvStartRow + 7);

        //напихиваем состав в обвязку, отступая шапку
        ParseStructure(worksheet, result, structureStartRow + 2);

        //Находим секцию датчиков, вытаскиваем их
        var (sensorsStartRow, sensorsEndRow) =
            _excelAddressingService.FindSectionRows(worksheet, "Датчики", obvStartRow);

        //находим секцию доп комплектующих (высчитываем)
        var (additionalStartRow, additionalLastRow) = (structureEndRow + 1, sensorsStartRow - 1);

        //напихиваем доп комплектующие в обвязку
        ParseAdditionalComponents(worksheet, result, additionalStartRow + 2, additionalLastRow);

        //Напихиваем датчики в обвязку
        ParseSensors(worksheet, result, sensorsStartRow + 2, sensorsEndRow);

        return (result, sensorsEndRow);
    }


    private void ParseStructure(IXLWorksheet worksheet, ObvyazkaDto target, int dataStartRow)
    {
        target.Line = _componentParser.ParseComponent(worksheet, dataStartRow);
        target.Armature = _componentParser.ParseComponent(worksheet, dataStartRow + 1);
        target.TreeSocket = _componentParser.ParseComponent(worksheet, dataStartRow + 2);
        target.KMCH = _componentParser.ParseComponent(worksheet, dataStartRow + 3);
    }


    private void ParseAdditionalComponents(IXLWorksheet worksheet, ObvyazkaDto target, int dataStartRow, int dataEndRow)
    {
        for (var row = dataStartRow; row <= dataEndRow; row++)
        {
            var parsedComponent = _componentParser.ParseComponent(worksheet, row);

            if (parsedComponent is null || parsedComponent.IsEmpty())
                continue;

            target.ObvAdditionalComponents.Add(parsedComponent);
        }
    }


    private void ParseSensors(IXLWorksheet worksheet, ObvyazkaDto target, int dataStartRow, int dataEndRow)
    {
        for (var row = dataStartRow; row <= dataEndRow; row++)
        {
            var parsedSensor = ParseSensor(worksheet, row);

            if (parsedSensor is null || parsedSensor.IsEmpty())
                continue;

            target.Sensors.Add(parsedSensor);
        }
    }


    private ObvSensorDto ParseSensor(IXLWorksheet worksheet, int row)
    {
        return new ObvSensorDto
        {
            Number = worksheet.Cell(row, _obvCellConfig.SensorNumberColumn).Value.ToString(),
            SensorType = worksheet.Cell(row, _obvCellConfig.SensorTypeColumn).Value.ToString(),
            KKS = worksheet.Cell(row, _obvCellConfig.SensorKKSColumn).Value.ToString(),
            MarkPlus = worksheet.Cell(row, _obvCellConfig.SensorMarkPlusColumn).Value.ToString(),
            MarkMinus = worksheet.Cell(row, _obvCellConfig.SensorMarkMinusColumn).Value.ToString(),
            Description = worksheet.Cell(row, _obvCellConfig.SensorDescriptionColumn).Value.ToString()
        };
    }

    private bool HasObvDataAt(IXLWorksheet worksheet, int row)
    {
        // обвязка считается начавшейся, если в строке номера/типа есть хоть что-то
        var number = worksheet.Cell(row, _obvCellConfig.ObvNumberColumn).GetString().Trim();
        var type = worksheet.Cell(row, _obvCellConfig.ObvTypeColumn).GetString().Trim();

        return !string.IsNullOrEmpty(number) || !string.IsNullOrEmpty(type);
    }
}
