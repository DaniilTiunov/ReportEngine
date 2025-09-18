using System.Windows.Input;

namespace ReportEngine.App.Commands;

public class MainWindowCommandProvider
{
    public ICommand OpenMainWindowCommand { get; set; }
    public ICommand OpenAllObvyazkiCommand { get; set; }
    public ICommand OpenTreeViewCommand { get; set; }
    public ICommand OpenFormedFramesCommand { get; set; }
    public ICommand CloseAppCommand { get; set; }
    public ICommand OpenAllUsersCommand { get; set; }
    public ICommand OpenAllCompaniesCommand { get; set; }
    public ICommand ChekDbConnectionCommand { get; set; }
    public ICommand ShowAllProjectsCommand { get; set; }
    public ICommand DeleteSelectedProjectCommand { get; set; }
    public ICommand EditProjectCommand { get; set; }
    public ICommand OpenAllDrainagesCommand { get; set; }
    public ICommand OpenAllSortamentsCommand { get; set; }
    public ICommand OpenSettingsWindow { get; set; }
    public ICommand OpenCalculationSettingsWindow { get; set; }
    public ICommand RecalculateProjectCommand { get; set; }
}