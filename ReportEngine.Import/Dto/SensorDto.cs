using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportEngine.Import.Dto;

public class SensorDto
{
    public string Number { get; set; } = "";

    public string SensorType { get; set; } = "";

    public string KKS { get; set; } = "";

    public string MarkPlus { get; set; } = "";
    public string MarkMinus { get; set; } = "";

    public string Description { get; set; } = "";
}
