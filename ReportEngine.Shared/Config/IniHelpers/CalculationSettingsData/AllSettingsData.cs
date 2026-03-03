using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportEngine.Shared.Config.IniHelpers.CalculationSettingsData
{
    public class AllSettingsData
    {
        public ElectricalSettingsData ElectricalSettings {  get; set; }
        public FrameSettingsData FrameSettings { get; set; }
        public HumanCostSettingsData HumanCostSettings { get; set; }
        public SandBlastSettingsData SandBlastSettings { get; set; }
        public StandSettingsData StandSettings { get; set; }
    }
}
