using ReportEngine.App.ViewModels;
using ReportEngine.App.Views;
using ReportEngine.App.Views.Controls;
using ReportEngine.App.Views.Settings;
using ReportEngine.App.Views.Windows;
using ReportEngine.Domain.Entities.Armautre;
using ReportEngine.Domain.Entities.Braces;
using ReportEngine.Domain.Entities.Drainage;
using ReportEngine.Domain.Entities.ElectricComponents;
using ReportEngine.Domain.Entities.ElectricSockets;
using ReportEngine.Domain.Entities.Frame;
using ReportEngine.Domain.Entities.Other;
using ReportEngine.Domain.Entities.Pipes;

namespace ReportEngine.App.Commands.Initializers;

public static class MainWindowCommandsInitializer
{
    public static void InitializeCommands(MainWindowViewModel vm)
    {
        if (vm == null)
            return;

        vm.MainWindowCommandProvider.OpenAuthWindowCommand =
            new RelayCommand(vm.OpenAuthWindowCommandExecuted<AuthWindow>, vm.CanAllCommandsExecute);

        vm.MainWindowCommandProvider.OpenAllUsersCommand =
            new RelayCommand(vm.OpenOthersWindowCommandExecuted<UsersView>, vm.CanAllCommandsExecute);

        vm.MainWindowCommandProvider.OpenAllObvyazkiCommand =
            new RelayCommand(vm.OpenOthersWindowCommandExecuted<ObvyazkiView>, vm.CanAllCommandsExecute);

        vm.MainWindowCommandProvider.OpenAllCompaniesCommand =
            new RelayCommand(vm.OpenOthersWindowCommandExecuted<CompanyView>, vm.CanAllCommandsExecute);

        vm.MainWindowCommandProvider.OpenFormedFramesCommand =
            new RelayCommand(vm.OpenOthersWindowCommandExecuted<FormedFrameView>, vm.CanAllCommandsExecute);

        vm.MainWindowCommandProvider.OpenAllSortamentsCommand =
            new RelayCommand(vm.OpenOthersWindowCommandExecuted<AllSortamentsView>, vm.CanAllCommandsExecute);

        vm.MainWindowCommandProvider.OpenSettingsWindow =
            new RelayCommand(vm.OpenOthersWindowCommandExecuted<SettingsWindow>, vm.CanAllCommandsExecute);

        vm.MainWindowCommandProvider.OpenAllSubjectsCommand =
            new RelayCommand(vm.OpenOthersWindowCommandExecuted<SubjectsView>, vm.CanAllCommandsExecute);

        vm.MainWindowCommandProvider.OpenCalculationSettingsWindow =
            new RelayCommand(vm.OpenOthersWindowCommandExecuted<CalculationSettingsWindow>, vm.CanAllCommandsExecute);

        vm.MainWindowCommandProvider.OpenTreeViewCommand =
            new RelayCommand(vm.OpenAnotherControlsCommandExecuted<TreeProjectView>, vm.CanAllCommandsExecute);

        vm.MainWindowCommandProvider.ChekDbConnectionCommand =
            new RelayCommand(vm.OnCheckDbConnectionCommandExecuted, vm.CanAllCommandsExecute);

        vm.MainWindowCommandProvider.ShowAllProjectsCommand =
            new RelayCommand(vm.OnShowAllProjectsCommandExecuted, vm.CanAllCommandsExecute);

        vm.MainWindowCommandProvider.DeleteSelectedProjectCommand =
            new RelayCommand(vm.OnDeleteSelectedProjectExecuted, vm.CanAllCommandsExecute);

        vm.MainWindowCommandProvider.OpenMainWindowCommand =
            new RelayCommand(vm.OnOpenMainWindowCommandExecuted, vm.CanAllCommandsExecute);

        vm.MainWindowCommandProvider.EditProjectCommand =
            new RelayCommand(vm.OnEditProjectCommandExecuted, vm.CanAllCommandsExecute);

        vm.MainWindowCommandProvider.OpenAllDrainagesCommand =
            new RelayCommand(vm.OpenOthersWindowCommandExecuted<FormedDrainagesView>, vm.CanAllCommandsExecute);

        vm.MainWindowCommandProvider.RecalculateProjectCommand =
            new RelayCommand(vm.OnRecalculateProjectCommandExecuted, vm.CanAllCommandsExecute);
    }

    public static void InitializeGenericCommands(MainWindowViewModel vm)
    {
        if (vm == null)
            return;

        vm.GenericEquipCommandProvider.OpenHeaterPipeCommand =
            new RelayCommand(vm.OnOpenGenericWindowCommandExecuted<HeaterPipe, HeaterPipe>, vm.CanAllCommandsExecute);
        vm.GenericEquipCommandProvider.OpenCarbonPipeCommand =
            new RelayCommand(vm.OnOpenGenericWindowCommandExecuted<CarbonPipe, CarbonPipe>, vm.CanAllCommandsExecute);
        vm.GenericEquipCommandProvider.OpenStainlessPipeCommand =
            new RelayCommand(vm.OnOpenGenericWindowCommandExecuted<StainlessPipe, StainlessPipe>,
                vm.CanAllCommandsExecute);

        vm.GenericEquipCommandProvider.OpenHeaterArmatureCommand =
            new RelayCommand(vm.OnOpenGenericWindowCommandExecuted<HeaterArmature, HeaterArmature>,
                vm.CanAllCommandsExecute);
        vm.GenericEquipCommandProvider.OpenCarbonArmatureCommand =
            new RelayCommand(vm.OnOpenGenericWindowCommandExecuted<CarbonArmature, CarbonArmature>,
                vm.CanAllCommandsExecute);
        vm.GenericEquipCommandProvider.OpenStainlessArmatureCommand =
            new RelayCommand(vm.OnOpenGenericWindowCommandExecuted<StainlessArmature, StainlessArmature>,
                vm.CanAllCommandsExecute);

        vm.GenericEquipCommandProvider.OpenCarbonSocketsCommand =
            new RelayCommand(vm.OnOpenGenericWindowCommandExecuted<CarbonSocket, CarbonSocket>,
                vm.CanAllCommandsExecute);
        vm.GenericEquipCommandProvider.OpenStainlessSocketsCommand =
            new RelayCommand(vm.OnOpenGenericWindowCommandExecuted<StainlessSocket, StainlessSocket>,
                vm.CanAllCommandsExecute);
        vm.GenericEquipCommandProvider.OpenHeaterSocketsCommand =
            new RelayCommand(vm.OnOpenGenericWindowCommandExecuted<HeaterSocket, HeaterSocket>,
                vm.CanAllCommandsExecute);

        vm.GenericEquipCommandProvider.OpenDrainageCommand =
            new RelayCommand(vm.OnOpenGenericWindowCommandExecuted<Drainage, Drainage>, vm.CanAllCommandsExecute);

        vm.GenericEquipCommandProvider.OpenFrameDetailsCommand =
            new RelayCommand(vm.OnOpenGenericWindowCommandExecuted<FrameDetail, FrameDetail>, vm.CanAllCommandsExecute);
        vm.GenericEquipCommandProvider.OpenPillarEquipCommand =
            new RelayCommand(vm.OnOpenGenericWindowCommandExecuted<PillarEqiup, PillarEqiup>, vm.CanAllCommandsExecute);
        vm.GenericEquipCommandProvider.OpenFrameRollCommand =
            new RelayCommand(vm.OnOpenGenericWindowCommandExecuted<FrameRoll, FrameRoll>, vm.CanAllCommandsExecute);

        vm.GenericEquipCommandProvider.OpenBoxesBracesommand =
            new RelayCommand(vm.OnOpenGenericWindowCommandExecuted<BoxesBrace, BoxesBrace>, vm.CanAllCommandsExecute);
        vm.GenericEquipCommandProvider.OpenDrainageBracesCommand =
            new RelayCommand(vm.OnOpenGenericWindowCommandExecuted<DrainageBrace, DrainageBrace>,
                vm.CanAllCommandsExecute);
        vm.GenericEquipCommandProvider.OpenSensorsBracesCommand =
            new RelayCommand(vm.OnOpenGenericWindowCommandExecuted<SensorBrace, SensorBrace>, vm.CanAllCommandsExecute);

        vm.GenericEquipCommandProvider.OpenCabelBoxeCommand =
            new RelayCommand(vm.OnOpenGenericWindowCommandExecuted<CabelBoxe, CabelBoxe>, vm.CanAllCommandsExecute);
        vm.GenericEquipCommandProvider.OpenCabelInputCommand =
            new RelayCommand(vm.OnOpenGenericWindowCommandExecuted<CabelInput, CabelInput>, vm.CanAllCommandsExecute);
        vm.GenericEquipCommandProvider.OpenCabelProductionCommand =
            new RelayCommand(vm.OnOpenGenericWindowCommandExecuted<CabelProduction, CabelProduction>,
                vm.CanAllCommandsExecute);
        vm.GenericEquipCommandProvider.OpenCabelProtectionCommand =
            new RelayCommand(vm.OnOpenGenericWindowCommandExecuted<CabelProtection, CabelProtection>,
                vm.CanAllCommandsExecute);
        vm.GenericEquipCommandProvider.OpenHeaterCommand =
            new RelayCommand(vm.OnOpenGenericWindowCommandExecuted<Heater, Heater>, vm.CanAllCommandsExecute);

        vm.GenericEquipCommandProvider.OpenConteinersCommand =
            new RelayCommand(vm.OnOpenGenericWindowCommandExecuted<Container, Container>, vm.CanAllCommandsExecute);
        vm.GenericEquipCommandProvider.OpenOthersCommand =
            new RelayCommand(vm.OnOpenGenericWindowCommandExecuted<Other, Other>, vm.CanAllCommandsExecute);
    }
}