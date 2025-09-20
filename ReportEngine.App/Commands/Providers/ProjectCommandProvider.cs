using System.Windows.Input;

namespace ReportEngine.App.Commands.Providers;

public class ProjectCommandProvider
{
    public ICommand SelectMaterialLineDialogCommand { get; set; }
    public ICommand SelectArmatureDialogCommand { get; set; }
    public ICommand SelectTreeSocketDialogCommand { get; set; }
    public ICommand SelectKMCHDialogCommand { get; set; }
    public ICommand SaveObvCommand { get; set; }
    public ICommand CreateNewCardCommand { get; set; }
    public ICommand AddNewStandCommand { get; set; }
    public ICommand SaveChangesCommand { get; set; }
    public ICommand AddFrameToStandCommand { get; set; }
    public ICommand AddDrainageToStandCommand { get; set; }
    public ICommand AddCustomDrainageToStandCommand { get; set; }
    public ICommand AddCustomElectricalComponentToStandCommand { get; set; }
    public ICommand AddCustomAdditionalEquipToStandCommand { get; set; }
    public ICommand SelectObvFromDialogCommand { get; set; }
    public ICommand CalculateProjectCommand { get; set; }
    public ICommand CreateSummaryReportCommand { get; set; }
    public ICommand OpenAllSortamentsDialogCommand { get; set; } // Новая команда для открытия окна ассортиментов
    public ICommand DeleteSelectedStandCommand { get; set; }
    public ICommand CreateMarkReportCommand { get; set; }
    public ICommand CreateNameplatesReportCommand { get; set; }
    public ICommand CreateProductionReportCommand {  get; set; }
    public ICommand CreateFinplanReportCommand { get; set; }
    public ICommand RemoveObvFromStandCommand { get; set; }
    public ICommand UpdateObvInStandCommand { get; set; }
    public ICommand CopyObvyazkaToStandsCommand { get; set; }
    
    public ICommand FillStandFieldsFromObvyazkaCommand { get; set; }
    public ICommand RemoveFrameStandCommand { get; set; }
    public ICommand CreateContainerReportCommand { get; set; }
    public ICommand SaveChangesInStandCommand { get; set; }
    public ICommand DeleteElectricalComponentFromStandCommand { get; set; }
    public ICommand UpdateElectricalComponentInStandCommand { get; set; }
    public ICommand DeleteAdditionalComponentFromStandCommand { get; set; }
    public ICommand UpdateAdditionalComponentInStandCommand { get; set; }
    public ICommand DeleteDrainageComponentFromStandCommand { get; set; }
    public ICommand UpdateDrainageComponentInStandCommand { get; set; }
    public ICommand CreateContainerBatchCommand { get; set; }
    public ICommand RefreshBatchesCommand { get; set; }
    public ICommand AddContainerToBatchCommand { get; set; }
    public ICommand DeleteContainerCommand { get; set; }
    public ICommand DeleteBatchCommand { get; set; }
    public ICommand AddStandToContainerCommand { get; set; }
    public ICommand RemoveStandFromContainerCommand { get; set; }
    public ICommand Bind(ICommand command, Action<object> execute, Func<object, bool> canExecute = null)
    {
        return new RelayCommand(execute, canExecute);
    }
}