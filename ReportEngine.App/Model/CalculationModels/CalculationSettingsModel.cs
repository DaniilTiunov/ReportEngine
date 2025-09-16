using ReportEngine.App.ViewModels;
using ReportEngine.Shared.CalculationSettings;
using ReportEngine.Shared.Config.IniHeleprs;

namespace ReportEngine.App.Model.CalculationModels
{
    public class CalculationSettingsModel : BaseViewModel
    {
        public HumanCostSettings HumanCostSettingsModel { get; set; } = new ();

        public void LoadDataFromIni()
        {
            HumanCostSettingsModel = CalculationSettingsManager.LoadHumanCostSettings();
        }
        public void SaveDataToIni()
        {
            CalculationSettingsManager.SaveHumanCostSettings(HumanCostSettingsModel);
        }
    }
}
