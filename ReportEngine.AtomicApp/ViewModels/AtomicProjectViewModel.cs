using System.Collections.ObjectModel;
using ReportEngine.AtomicApp.Commands.Initializers;
using ReportEngine.AtomicApp.Commands.Providers;
using ReportEngine.AtomicApp.Extensions;
using ReportEngine.AtomicApp.ViewModels.Abstracts;
using ReportEngine.AtomicServices.Models;
using ReportEngine.AtomicServices.Services;

namespace ReportEngine.AtomicApp.ViewModels;

public class AtomicProjectViewModel : BaseViewModel
{
    private readonly AtomicProjectService _projectService;

    private AtomicProjectModel _currentProject = new();

    public AtomicProjectViewModel(AtomicProjectService projectService)
    {
        _projectService = projectService;

        InitializeCommands();
    }

    public ProjectCommandProvider CommandProvider { get; set; } = new();

    public AtomicProjectModel CurrentProject
    {
        get => _currentProject;
        set => Set(ref _currentProject, value);
    }

    public ObservableCollection<AtomicProjectModel> AllProjects { get; set; } = new();

    public void InitializeCommands()
    {
        ProjectCommandsInitializer.InitializeCommands(this);
    }

    public bool CommandsCanExecute(object obj)
    {
        return true;
    }

    public async Task OnAddNewProjectCommandExecuted(object obj)
    {
        await _projectService.AddNewProjectAsync(CurrentProject);
    }

    public async Task OnGetProjectCommandExecuted(object obj)
    {
        var allProjects = await _projectService.GetAllProjectsAsync();

        AllProjects.ReplaceWith(allProjects);
    }
}
