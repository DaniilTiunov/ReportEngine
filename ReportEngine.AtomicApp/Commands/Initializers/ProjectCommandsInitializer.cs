using ReportEngine.AtomicApp.Commands.BaseCommands.AsyncCommands;
using ReportEngine.AtomicApp.ViewModels;

namespace ReportEngine.AtomicApp.Commands.Initializers;

public static class ProjectCommandsInitializer
{
    public static void InitializeCommands(AtomicProjectViewModel vm)
    {
        if (vm == null)
            return;

        vm.CommandProvider.AddNewProjectAsyncCommand = new AsyncRelayCommand
            (vm.OnAddNewProjectCommandExecuted, vm.CommandsCanExecute);

        vm.CommandProvider.GetProjectAsyncCommand = new AsyncRelayCommand
            (vm.OnGetProjectCommandExecuted, vm.CommandsCanExecute);
    }
}
