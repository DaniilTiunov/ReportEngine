using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportEngine.Import.Dto;

public class ObvyazkaDto
{
    ComponentDto Line { get; set; }
    ComponentDto Armature { get; set; }
    ComponentDto TreeSocket { get; set; }
    ComponentDto KMCH { get; set; }
    List<ComponentDto> ObvAdditionalComponents { get; set; }
    List<SensorDto> Sensors { get; set; }




}
