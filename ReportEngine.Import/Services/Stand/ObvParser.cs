using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Npgsql.EntityFrameworkCore.PostgreSQL.Query.ExpressionTranslators.Internal;
using ReportEngine.Import.Configuration;
using ReportEngine.Import.Dto;
using ReportEngine.Import.Services.Excel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportEngine.Import.Services.Stand;

public class ObvParser
{
    private readonly ObvCellConfig _obvCellConfig = new();

    private readonly ComponentParser _componentParser;

    private readonly ExcelAddressingService _excelAddressingService;

    public ObvParser(ExcelAddressingService excelAddressingService,ComponentParser componentParser)
    {
        _excelAddressingService = excelAddressingService;
        _componentParser = componentParser;
    }

    public List<ObvyazkaDto> Parse(IXLWorksheet worksheet)
    {

        var results = new List<ObvyazkaDto>();



        var allObvTitleRow = _excelAddressingService.FindSectionStartRow(worksheet, "ОБВЯЗКИ СТЕНДА");

        //начинаем с первой
        var currentObvStartRow = allObvTitleRow + 1;
        ObvyazkaDto obv = null;

        //идем до тех пор, пока не встретим пустую обвязку
        do
        {
            try
            {
                (obv, currentObvStartRow) = ParseObv(worksheet, currentObvStartRow);
                if (obv?.IsEmpty() == false)
                {
                    results.Add(obv);
                }

                currentObvStartRow += 2;
            }
            catch
            {
                break;
            }
           




        }
        while (obv?.IsEmpty() == false);
        

        return results;
    }

    public (ObvyazkaDto obv, int lastObvRow) ParseObv(IXLWorksheet worksheet, int obvStartRow)
    {
        var result = new ObvyazkaDto();

        //вытаскиваем номер и тип обвязки
        var commonObvInfoRow = obvStartRow + 1;
        result.Number = worksheet.Cell(commonObvInfoRow, _obvCellConfig.ObvNumberColumn).Value.ToString();
        result.ObvType = worksheet.Cell(commonObvInfoRow, _obvCellConfig.ObvTypeColumn).Value.ToString();

        //находим секцию состав целиком
        var (structureStartRow, structureEndRow) = (obvStartRow + 2, obvStartRow + 7);

        //напихиваем состав в обвязку, отступая шапку
        ParseStructure(worksheet, result, structureStartRow + 2);

        //Находим секцию датчиков, вытаскиваем их
        var (sensorsStartRow, sensorsEndRow) = _excelAddressingService.FindSectionRows(worksheet, "Датчики", obvStartRow);

        //находим секцию доп комплектующих (высчитываем)
        var (additionalStartRow, additionaLastRow) = (structureEndRow + 1, sensorsStartRow - 1);

        //напихиваем доп комплектующие в обвязку
        ParseAdditionalComponents(worksheet, result, additionalStartRow + 2, additionaLastRow);

        //Напихиваем датчики в обвязку
        ParseSensors(worksheet, result, sensorsStartRow + 2, sensorsEndRow);

        return (result, sensorsEndRow);
    }


    private void ParseStructure(IXLWorksheet worksheet, ObvyazkaDto target,int dataStartRow)
    {
        target.Line = _componentParser.ParseComponent(worksheet, dataStartRow);
        target.Armature = _componentParser.ParseComponent(worksheet, dataStartRow + 1);
        target.TreeSocket = _componentParser.ParseComponent(worksheet, dataStartRow + 2);
        target.KMCH = _componentParser.ParseComponent(worksheet, dataStartRow + 3);
    }




    private void ParseAdditionalComponents(IXLWorksheet worksheet, ObvyazkaDto target, int dataStartRow, int dataEndRow)
    {
        for (int row = dataStartRow; row <= dataEndRow; row++)
        {
            var parsedComponent = _componentParser.ParseComponent(worksheet, row);

            if (parsedComponent is null || parsedComponent.IsEmpty())
                continue;

            target.ObvAdditionalComponents.Add(parsedComponent);
        }
    }


    private void ParseSensors(IXLWorksheet worksheet, ObvyazkaDto target, int dataStartRow, int dataEndRow)
    {
        for (int row = dataStartRow; row <= dataEndRow; row++)
        {

            var parsedSensor = ParseSensor(worksheet, row);


            if (parsedSensor is null || parsedSensor.IsEmpty())
                continue;

            target.Sensors.Add(parsedSensor);
        }
    }




    private ObvSensorDto ParseSensor(IXLWorksheet worksheet,int row)
    {
        return new ObvSensorDto()
        {
            Number = worksheet.Cell(row, _obvCellConfig.SensorNumberColumn).Value.ToString(),
            SensorType = worksheet.Cell(row, _obvCellConfig.SensorTypeColumn).Value.ToString(),
            KKS = worksheet.Cell(row, _obvCellConfig.SensorKKSColumn).Value.ToString(),
            MarkPlus = worksheet.Cell(row, _obvCellConfig.SensorMarkPlusColumn).Value.ToString(),
            MarkMinus = worksheet.Cell(row, _obvCellConfig.SensorMarkMinusColumn).Value.ToString(),
            Description = worksheet.Cell(row, _obvCellConfig.SensorDescriptionColumn).Value.ToString()
        };
    }
}
