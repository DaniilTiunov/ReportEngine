using System.Windows.Input;

namespace ReportEngine.App.Commands.Providers;

public class ProjectCommandProvider
{
    public ICommand UpdateStandsAfterEquipsCommand { get; set; }
    public ICommand SelectMaterialLineDialogCommand { get; set; }
    public ICommand SelectArmatureDialogCommand { get; set; }
    public ICommand SelectTreeSocketDialogCommand { get; set; }
    public ICommand SelectKMCHDialogCommand { get; set; }
    public ICommand SaveObvCommand { get; set; }
    public ICommand CreateNewCardCommand { get; set; }
    public ICommand AddNewStandCommand { get; set; }
    public ICommand CopyStandsCommand { get; set; }
    public ICommand SaveChangesCommand { get; set; }
    public ICommand AddFrameToStandCommand { get; set; }
    public ICommand AddDrainageToStandCommand { get; set; }
    public ICommand AddCustomDrainageToStandCommand { get; set; }
    public ICommand AddCustomElectricalComponentToStandCommand { get; set; }
    public ICommand AddCustomAdditionalEquipToStandCommand { get; set; }
    public ICommand SelectObvFromDialogCommand { get; set; }
    public ICommand CalculateProjectCommand { get; set; }
    public ICommand CreateComponentsListReportCommand { get; set; }
    public ICommand CreateSummaryReportCommand { get; set; }
    public ICommand OpenAllSortamentsDialogCommand { get; set; }
    public ICommand DeleteSelectedStandCommand { get; set; }
    public ICommand CreateMarkReportCommand { get; set; }
    public ICommand CreateMarkReportAsyncCommand { get; set; }
    public ICommand CreateNameplatesReportCommand { get; set; }
    public ICommand CreateProductionReportCommand { get; set; }
    public ICommand CreateFinplanReportCommand { get; set; }
    public ICommand RemoveObvFromStandCommand { get; set; }
    public ICommand UpdateObvInStandCommand { get; set; }
    public ICommand CopyObvyazkaToStandsCommand { get; set; }
    public ICommand CreatePassportReportCommand { get; set; }
    public ICommand CreateTechnologicalCardsReportCommand { get; set; }
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
    public ICommand UpdateContainerCommand { get; set; }
    public ICommand DeleteBatchCommand { get; set; }
    public ICommand AddStandToContainerCommand { get; set; }
    public ICommand RemoveStandFromContainerCommand { get; set; }
    public ICommand CreateContainerReportCommandAsync { get; set; }
    public ICommand ShowCompanyDialogCommand { get; set; }
    public ICommand ShowFrameDialogCommand { get; set; }
    public ICommand ShowSubjectDialogCommand { get; set; }
    public ICommand UpdateUICommand { get; set; }
    public ICommand RenumerateStandsCommand { get; set; }
    public ICommand OpenObvSettingsWindowCommand { get; set; }
    public ICommand OpenCreateNewStandCommand { get; set; }
    public ICommand OpenEditStandCommand { get; set; }
    public ICommand FillObvFieldsTiEditCommand { get; set; }
    public ICommand DeleteAdditionalEquipFromObvCommand {  get; set; }
    public ICommand UpdateAdditionalEquipFromObvCommand { get; set; }
    public ICommand AdditionalTestCommand { get; set;  }
    public ICommand SaveAllChangesInComponentsCommand {  get; set; }
    public ICommand RenumerateObvInStandCommand { get; set; }

    // Команды по отчтам выбранных стендов
    public ICommand SelectedSummaryReportCommand { get; set; }
    public ICommand SelectedComponentReportCommand { get; set; }
    public ICommand SelectedMarksReportCommand { get; set; }
    public ICommand SelectedNamePlatesReportCommand { get; set; }
    public ICommand SelectedContainerReportCommand { get; set; }
    public ICommand SelectedProductionReportCommand { get; set; }
    public ICommand SelectedFinPlaneReportCommand { get; set; }
    public ICommand SelectedPassportReportCommand { get; set; }
    public ICommand SelectedTechnoCardsReportCommand { get; set; }
    public ICommand OnAddStandFromAllStandsCommand {  get; set; }
}
