using ClosedXML.Excel;
using ReportEngine.Import.Configuration;
using ReportEngine.Import.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportEngine.Import.Services.Stand;

public class ContainerParser
{
    private readonly ContainerCellConfig _containerCellConfig = new();


    public List<ContainerDto> Parse(IXLWorksheet worksheet)
    {
        var startDataRow = 2;

        var results = new List<ContainerDto>();



    }
}
