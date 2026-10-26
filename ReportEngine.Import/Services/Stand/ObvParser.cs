using ClosedXML.Excel;
using ReportEngine.Import.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportEngine.Import.Services.Stand;

public class ObvParser
{
    public ObvyazkaDto ParseObv(IXLWorksheet worksheet)
    {
        return new ObvyazkaDto();
    }

    private List<ComponentDto> ParseAdditionalComponents(IXLWorksheet worksheet)
    {
        var obvAdditionalComponents = new List<ComponentDto>();

        return obvAdditionalComponents;
    }


    private List<SensorDto> ParseSensors(IXLWorksheet worksheet)
    {
        var parsedSensors = new List<SensorDto>();

        return parsedSensors;
    }
}
