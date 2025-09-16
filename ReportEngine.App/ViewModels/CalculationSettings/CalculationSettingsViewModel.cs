using ReportEngine.App.Model.CalculationModels;

namespace ReportEngine.App.ViewModels.CalculationSettings
{
    public class CalculationSettingsViewModel : BaseViewModel
    {
        public HumanCostSettingsModel HumanCosts { get; set; } = new();

        public CalculationSettingsViewModel() 
        {
            LoadSettings();
        }

        public void LoadSettings()
        {
            HumanCosts.LoadDataFromIni();
        }

        public void SaveSettings()
        {
            HumanCosts.SaveDataToIni();
        }
    }
}
