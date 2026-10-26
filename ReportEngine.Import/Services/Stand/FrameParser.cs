using ClosedXML.Excel;
using ReportEngine.Import.Configuration;
using ReportEngine.Import.Dto;
using ReportEngine.Import.Services.Excel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportEngine.Import.Services.Stand;

public class FrameParser
{
    private readonly FrameCellConfig _cellConfig = new();

    private readonly ExcelAddressingService _excelAddressingService;


    private readonly string _categoryName = "РАМЫ";

    public FrameParser(ExcelAddressingService excelAddressingService)
    {
        _excelAddressingService = excelAddressingService;
    }

    public List<FrameDto> Parse(IXLWorksheet worksheet)
    {
        var frames = new List<FrameDto>();

        var (startRow, lastRow) = _excelAddressingService.FindSectionRows(worksheet, _categoryName);


        for (var row = startRow + 2; row <= lastRow; row++)
        {
            var frame = ParseFrame(worksheet, row);

            if (frame.IsEmpty())
                continue;

            frames.Add(frame);
        }

        return frames;
    }




    private FrameDto ParseFrame(IXLWorksheet worksheet, int row)
    {
        return new FrameDto
        {
            Name = worksheet
                .Cell(row, _cellConfig.ColumnName)
                .GetString(),

            FrameType = worksheet
                .Cell(row, _cellConfig.ColumnFrameType)
                .GetString(),

            Width = worksheet
                .Cell(row, _cellConfig.ColumnWidth)
                .GetString(),

            Height = worksheet
                .Cell(row, _cellConfig.ColumnHeight)
                .GetString(),

            Depth = worksheet
                .Cell(row, _cellConfig.ColumnDepth)
                .GetString(),

            Weight = worksheet
                .Cell(row, _cellConfig.ColumnWeight)
                .GetString(),

            Description = worksheet
                .Cell(row, _cellConfig.ColumnDescription)
                .GetString()
        };
    }


}
