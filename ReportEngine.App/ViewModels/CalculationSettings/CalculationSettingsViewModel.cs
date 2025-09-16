using ReportEngine.App.Model.CalculationModels;

namespace ReportEngine.App.ViewModels.CalculationSettings
{
    public class CalculationSettingsViewModel : BaseViewModel
    {
        public HumanCostSettingsModel HumanCosts { get; set; } = new();

        public CalculationSettingsViewModel() 
        {
            
        }

        public async Task LoadSettings()
        {
            await HumanCosts.LoadDataFromIniAsync();
        }

        public async Task SaveSettings()
        {
            await HumanCosts.SaveDataToIniAsync();
        }
    }
}
