using CommunityToolkit.Mvvm.Input;
using ReportEngine.App.ViewModels;

namespace ReportEngine.App.Commands.Initializers;

public static class ProjectCommandsInitializer
{
    public static void InitializeCommands(ProjectViewModel vm)
    {
        if (vm == null)
            return;

        vm.ProjectCommandProvider.UpdateStandsAfterEquipsCommand =
            new AsyncRelayCommand(vm.OnUpdateStandsAfterEquipsCommandExecuted);

        vm.ProjectCommandProvider.CreateNewCardCommand =
            new AsyncRelayCommand(vm.OnCreateNewCardCommandExecuted);

        vm.ProjectCommandProvider.AddNewStandCommand =
            new AsyncRelayCommand(vm.OnAddNewStandCommandExecuted);

        vm.ProjectCommandProvider.CopyStandsCommand =
            new AsyncRelayCommand(vm.OnCopyStandsCommandExecuted);

        vm.ProjectCommandProvider.SaveChangesCommand =
            new AsyncRelayCommand(vm.OnSaveChangesCommandExecuted);

        vm.ProjectCommandProvider.AddFrameToStandCommand =
            new AsyncRelayCommand(vm.OnAddFrameToStandExecuted);

        vm.ProjectCommandProvider.AddDrainageToStandCommand =
            new AsyncRelayCommand(vm.OnAddDrainageToStandExecuted);

        vm.ProjectCommandProvider.SelectObvFromDialogCommand =
            new RelayCommand(vm.OnSelectObvCommandExecuted, vm.CanAllCommandsExecute);

        vm.ProjectCommandProvider.CopyObvyazkaToStandsCommand =
            new AsyncRelayCommand(vm.OnCopyObvyazkaToStandsCommandExecuted);

        vm.ProjectCommandProvider.CalculateProjectCommand =
            new AsyncRelayCommand(vm.OnCalculateProjectCommandExecuted);

        vm.ProjectCommandProvider.DeleteSelectedStandCommand =
            new AsyncRelayCommand(vm.OnDeleteSelectedStandFromProjectExecuted);

        vm.ProjectCommandProvider.RemoveObvFromStandCommand =
            new AsyncRelayCommand(vm.OnRemoveObvCommandExecuted);

        vm.ProjectCommandProvider.OpenAllSortamentsDialogCommand =
            new RelayCommand(vm.OnOpenAllSortamentsDialogExecuted, vm.CanAllCommandsExecute);

        vm.ProjectCommandProvider.SaveChangesInStandCommand =
            new AsyncRelayCommand(vm.OnSaveChangesInStandCommandExecuted);

        vm.ProjectCommandProvider.DeleteElectricalComponentFromStandCommand =
            new AsyncRelayCommand(vm.OnDeleteElectricalComponentFromStandCommandExecuted);

        vm.ProjectCommandProvider.UpdateElectricalComponentInStandCommand =
            new AsyncRelayCommand(vm.OnUpdateElectricalComponentInStandCommandExecuted);

        vm.ProjectCommandProvider.DeleteAdditionalComponentFromStandCommand =
            new AsyncRelayCommand(vm.OnDeleteAdditionalComponentFromStandCommandExecuted);

        vm.ProjectCommandProvider.UpdateAdditionalComponentInStandCommand =
            new AsyncRelayCommand(vm.OnUpdateAdditionalComponentInStandCommandExecuted);

        vm.ProjectCommandProvider.DeleteDrainageComponentFromStandCommand =
            new AsyncRelayCommand(vm.OnDeleteDrainageComponentFromStandCommandExecuted);

        vm.ProjectCommandProvider.RemoveFrameStandCommand =
            new AsyncRelayCommand(vm.OnRemoveFrameFromStandCommandExecuted);

        vm.ProjectCommandProvider.UpdateDrainageComponentInStandCommand =
            new AsyncRelayCommand(vm.OnUpdateDrainageComponentInStandCommandExecuted);

        vm.ProjectCommandProvider.SaveObvCommand =
            new AsyncRelayCommand(vm.OnAddObvCommandExecuted);

        vm.ProjectCommandProvider.FillStandFieldsFromObvyazkaCommand =
            new AsyncRelayCommand(vm.OnEditObvSettingsCommandExecuted);

        vm.ProjectCommandProvider.UpdateObvInStandCommand =
            new AsyncRelayCommand(vm.OnUpdateObvInStandCommandExecuted);

        vm.ProjectCommandProvider.FillMarkInObvCommand =
            new AsyncRelayCommand(vm.OnFillMarkInObvCommandExecuted);

        vm.ProjectCommandProvider.ShowCompanyDialogCommand =
            new AsyncRelayCommand(vm.OnShowCompanyDialogExecuted);

        vm.ProjectCommandProvider.ShowFrameDialogCommand =
            new AsyncRelayCommand(vm.OnShowFrameDialogExecuted);

        vm.ProjectCommandProvider.ShowSubjectDialogCommand =
            new AsyncRelayCommand(vm.OnShowSubjectDialogExecuted);

        vm.ProjectCommandProvider.RenumerateStandsCommand =
            new AsyncRelayCommand(vm.OnRenumerateStandsCommandExecuted);

        vm.ProjectCommandProvider.OpenObvSettingsWindowCommand =
            new AsyncRelayCommand(vm.OnOpenObvSettingsWindowCommandExecuted);

        vm.ProjectCommandProvider.OpenCreateNewStandCommand =
            new AsyncRelayCommand(vm.OnOpenCreateNewStandCommandExecuted);

        vm.ProjectCommandProvider.OpenEditStandCommand =
            new AsyncRelayCommand(vm.OnOpenEditStandCommandExecuted);

        vm.ProjectCommandProvider.FillObvFieldsTiEditCommand =
            new AsyncRelayCommand(vm.OnFillObvFieldsCommandExecuted);

        vm.ProjectCommandProvider.DeleteAdditionalEquipFromObvCommand =
            new AsyncRelayCommand(vm.OnDeleteAdditionalEquipFromObvCommandExecuted);

        vm.ProjectCommandProvider.UpdateAdditionalEquipFromObvCommand =
            new AsyncRelayCommand(vm.OnUpdateAdditionalEquipFromObvCommandExecuted);

        vm.ProjectCommandProvider.AdditionalTestCommand =
            new AsyncRelayCommand(vm.OnAdditionalTestCommandExecuted);

        vm.ProjectCommandProvider.SaveAllChangesInComponentsCommand =
            new AsyncRelayCommand(vm.OnSaveAllChangesInComponentsCommandExecuted);

        vm.ProjectCommandProvider.RenumerateObvInStandCommand =
            new RelayCommand(vm.OnRenumerateObvInStandAsyncCommandExecuted, vm.CanAllCommandsExecute);

        vm.ProjectCommandProvider.OnAddStandFromAllStandsCommand =
            new AsyncRelayCommand(vm.OnAddStandFromAllStandsCommandExecuted);

        //отчеты по выбранным стендам
        vm.ProjectCommandProvider.SelectedStandsSummaryReportCommand =
            new AsyncRelayCommand(vm.OnCreateSelectedStandsSummaryReportCommandExecuted);

        vm.ProjectCommandProvider.SelectedStandsComponentReportCommand =
            new AsyncRelayCommand(vm.OnCreateSelectedStandsComponentsListReportCommandExecuted);

        vm.ProjectCommandProvider.SelectedStandsNamePlatesReportCommand =
            new AsyncRelayCommand(vm.OnCreateSelectedStandsNameplatesReportCommandExecuted);

        vm.ProjectCommandProvider.SelectedStandsMarksReportCommand =
            new AsyncRelayCommand(vm.OnCreateSelectedStandsMarksReportCommandExecuted);

        vm.ProjectCommandProvider.SelectedStandsContainerReportCommand =
            new AsyncRelayCommand(vm.OnCreateSelectedStandsContainerReportCommandExecuted);

        vm.ProjectCommandProvider.SelectedStandsProductionReportCommand =
            new AsyncRelayCommand(vm.OnCreateSelectedStandsProductionReportCommandExecuted);

        vm.ProjectCommandProvider.SelectedStandsFinPlaneReportCommand =
            new AsyncRelayCommand(vm.OnCreateSelectedStandsFinplanReportCommandExecuted);

        vm.ProjectCommandProvider.SelectedStandsPassportReportCommand =
            new AsyncRelayCommand(vm.OnCreateSelectedStandsPassportReportCommandExecuted);

        vm.ProjectCommandProvider.SelectedStandsTechnoCardsReportCommand =
            new AsyncRelayCommand(vm.OnCreateSelectedStandsTechnologicalCardsCommandExecute);

        vm.ProjectCommandProvider.DeleteSelectedStandsCommand =
            new AsyncRelayCommand(vm.OnDeleteSelectedStandsCommandExecuted);
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