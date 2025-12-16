using System.Collections.ObjectModel;
using ReportEngine.App.ViewModels;
using ReportEngine.Domain.Entities;

namespace ReportEngine.App.Model;

public class MainWindowModel : BaseViewModel
{
    #region Приватные поля

    private string _connectionStatusMessage;
    private ObservableCollection<ProjectInfo> _allProjects = new();
    private ProjectInfo _selectedProject;

    #endregion Приватные поля

    #region Публичные поля для привязки

    public ProjectInfo SelectedProject
    {
        get => _selectedProject;
        set => Set(ref _selectedProject, value);
    }

    public string ConnectionStatusMessage
    {
        get => _connectionStatusMessage;
        set => Set(ref _connectionStatusMessage, value);
    }

    public bool IsConnected { get; set; }

    public ObservableCollection<ProjectInfo> AllProjects
    {
        get => _allProjects;
        set => Set(ref _allProjects, value);
    }

    #endregion Публичные поля для привязки
}
