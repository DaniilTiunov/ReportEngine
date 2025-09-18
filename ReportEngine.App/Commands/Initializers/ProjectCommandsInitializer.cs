using ReportEngine.App.Commands.Providers;
using ReportEngine.App.ViewModels;
using System.Windows.Input;

namespace ReportEngine.App.Commands.Initializers
{
    public static class ProjectCommandsInitializer
    {
        public static void InitializeCommands(ProjectViewModel vm)
        {
            if (vm == null)
                return;

            vm.ProjectCommandProvider.CreateNewCardCommand =
                new RelayCommand(vm.OnCreateNewCardCommandExecuted, vm.CanAllCommandsExecute);

            vm.ProjectCommandProvider.AddNewStandCommand =
                new RelayCommand(vm.OnAddNewStandCommandExecuted, vm.CanAllCommandsExecute);

            vm.ProjectCommandProvider.SaveChangesCommand =
                new RelayCommand(vm.OnSaveChangesCommandExecuted, vm.CanAllCommandsExecute);

            vm.ProjectCommandProvider.AddFrameToStandCommand =
                new RelayCommand(vm.OnAddFrameToStandExecuted, vm.CanAllCommandsExecute);

            vm.ProjectCommandProvider.AddDrainageToStandCommand =
                new RelayCommand(vm.OnAddDrainageToStandExecuted, vm.CanAllCommandsExecute);

            vm.ProjectCommandProvider.AddCustomDrainageToStandCommand =
                new RelayCommand(vm.OnAddCustomDrainageToStandExecuted, vm.CanAllCommandsExecute);

            vm.ProjectCommandProvider.AddCustomElectricalComponentToStandCommand =
                new RelayCommand(vm.OnAddCustomElectricalComponentToStandExecuted, vm.CanAllCommandsExecute);

            vm.ProjectCommandProvider.AddCustomAdditionalEquipToStandCommand =
                new RelayCommand(vm.OnAddCustomAdditionalEquipToStandExecuted, vm.CanAllCommandsExecute);

            vm.ProjectCommandProvider.SelectObvFromDialogCommand =
                new RelayCommand(vm.OnSelectObvCommandExecuted, vm.CanAllCommandsExecute);

            vm.ProjectCommandProvider.CalculateProjectCommand =
                new RelayCommand(vm.OnCalculateProjectCommandExecuted, vm.CanAllCommandsExecute);

            vm.ProjectCommandProvider.CreateSummaryReportCommand =
                new RelayCommand(vm.OnCreateSummaryReportCommandExecuted, vm.CanAllCommandsExecute);

            vm.ProjectCommandProvider.OpenAllSortamentsDialogCommand =
                new RelayCommand(vm.OnOpenAllSortamentsDialogExecuted, vm.CanAllCommandsExecute);

            vm.ProjectCommandProvider.CreateMarkReportCommand =
                new RelayCommand(vm.OnCreateMarksReportCommandExecuted, vm.CanAllCommandsExecute);

            vm.ProjectCommandProvider.CreateNameplatesReportCommand =
                new RelayCommand(vm.OnCreateNameplatesReportCommandExecuted, vm.CanAllCommandsExecute);

            vm.ProjectCommandProvider.DeleteSelectedStandCommand =
                new RelayCommand(vm.OnDeleteSelectedStandFromProjectExecuted, vm.CanAllCommandsExecute);

            vm.ProjectCommandProvider.RemoveObvFromStandCommand =
                new RelayCommand(vm.OnRemoveObvCommandExecuted, vm.CanAllCommandsExecute);

            vm.ProjectCommandProvider.CreateContainerReportCommand =
                new RelayCommand(vm.OnCreateContainerReportCommandExecuted, vm.CanAllCommandsExecute);

            vm.ProjectCommandProvider.SaveChangesInStandCommand =
                new RelayCommand(vm.OnSaveChangesInStandCommandExecuted, vm.CanAllCommandsExecute);

            vm.ProjectCommandProvider.DeleteElectricalComponentFromStandCommand =
                new RelayCommand(vm.OnDeleteElectricalComponentFromStandCommandExecuted, vm.CanAllCommandsExecute);

            vm.ProjectCommandProvider.UpdateElectricalComponentInStandCommand =
                new RelayCommand(vm.OnUpdateElectricalComponentInStandCommandExecuted, vm.CanAllCommandsExecute);

            vm.ProjectCommandProvider.DeleteAdditionalComponentFromStandCommand =
                new RelayCommand(vm.OnDeleteAdditionalComponentFromStandCommandExecuted, vm.CanAllCommandsExecute);

            vm.ProjectCommandProvider.UpdateAdditionalComponentInStandCommand =
                new RelayCommand(vm.OnUpdateAdditionalComponentInStandCommandExecuted, vm.CanAllCommandsExecute);

            vm.ProjectCommandProvider.DeleteDrainageComponentFromStandCommand =
                new RelayCommand(vm.OnDeleteDrainageComponentFromStandCommandExecuted, vm.CanAllCommandsExecute);
            
            vm.ProjectCommandProvider.RemoveFrameStandCommand =
                new RelayCommand(vm.OnRemoveFrameFromStandCommandExecuted, vm.CanAllCommandsExecute);

            vm.ProjectCommandProvider.UpdateDrainageComponentInStandCommand =
                new RelayCommand(vm.OnUpdateDrainageComponentInStandCommandExecuted, vm.CanAllCommandsExecute);

            vm.ProjectCommandProvider.SaveObvCommand =
                new RelayCommand(vm.OnSaveObvCommandExecuted, vm.CanAllCommandsExecute);
            
            vm.ProjectCommandProvider.FillStandFieldsFromObvyazkaCommand =
                new RelayCommand(vm.OnFillStandFieldsFromObvyazkaCommandExecuted, vm.CanAllCommandsExecute);
            
            vm.ProjectCommandProvider.UpdateObvInStandCommand =
                new RelayCommand(vm.OnUpdateObvInStandCommandExecuted, vm.CanAllCommandsExecute);

            vm.ProjectCommandProvider.CreateContainerBatchCommand =
                new RelayCommand(vm.OnCreateContainerStandCommandExecuted, vm.CanAllCommandsExecute);

            vm.ProjectCommandProvider.RefreshBatchesCommand = 
                new RelayCommand(vm.OnRefreshBatchesCommandCommandExecuted, vm.CanAllCommandsExecute);

            vm.ProjectCommandProvider.AddContainerToBatchCommand = 
                new RelayCommand(vm.OnAddContainerToBatchCommandExecuted, vm.CanAllCommandsExecute);

            vm.ProjectCommandProvider.DeleteContainerCommand = 
                new RelayCommand(vm.OnDeleteContainerCommandExecuted, vm.CanAllCommandsExecute);

            vm.ProjectCommandProvider.DeleteBatchCommand =
                new RelayCommand(vm.OnDeleteBatchCommandExecuted, vm.CanAllCommandsExecute);

            vm.ProjectCommandProvider.AddStandToContainerCommand = 
                new RelayCommand(vm.OnAddStandToContainerCommandExecuted, vm.CanAllCommandsExecute);

            vm.ProjectCommandProvider.RemoveStandFromContainerCommand = 
                new RelayCommand(vm.OnRemoveStandFromContainerCommandExecuted, vm.CanAllCommandsExecute);
        }

        public static void InitializeGenericCommands(ProjectViewModel vm)
        {
            vm.ProjectCommandProvider.SelectMaterialLineDialogCommand =
                new RelayCommand(vm.OnSelectMaterialFromDialogCommandExecuted, vm.CanAllCommandsExecute);

            vm.ProjectCommandProvider.SelectArmatureDialogCommand =
                new RelayCommand(vm.OnSelectArmatureFromDialogCommandExecuted, vm.CanAllCommandsExecute);

            vm.ProjectCommandProvider.SelectKMCHDialogCommand =
                new RelayCommand(vm.OnSelectKMCHFromDialogCommandExecuted, vm.CanAllCommandsExecute);

            vm.ProjectCommandProvider.SelectTreeSocketDialogCommand =
                new RelayCommand(vm.OnSelectTreeSocketFromDialogCommandExecuted, vm.CanAllCommandsExecute);
        }
    }
}