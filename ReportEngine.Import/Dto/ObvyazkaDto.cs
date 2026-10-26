using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportEngine.Import.Dto;

public class ObvyazkaDto
{
    public string Number { get; set; }

    public string ObvType { get; set; } 
    public ComponentDto Line { get; set; } = new(); 
    public ComponentDto Armature { get; set; } = new();
    public ComponentDto TreeSocket { get; set; } = new();
    public ComponentDto KMCH { get; set; } = new();
    public List<ComponentDto> ObvAdditionalComponents { get; set; } = new();
    public List<ObvSensorDto> Sensors { get; set; } = new();


    public bool IsEmpty()
    {
        return string.IsNullOrWhiteSpace(Number) || string.IsNullOrWhiteSpace(ObvType);
    }




}
