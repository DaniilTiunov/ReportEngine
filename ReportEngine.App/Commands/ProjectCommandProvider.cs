using System.Windows.Input;

namespace ReportEngine.App.Commands;

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
}