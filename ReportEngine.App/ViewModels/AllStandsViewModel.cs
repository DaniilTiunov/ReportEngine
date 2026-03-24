using System.Collections.ObjectModel;
using ReportEngine.App.Extensions;
using ReportEngine.App.Model.StandsModel;
using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;

namespace ReportEngine.App.ViewModels
{
    public class AllStandsViewModel : BaseViewModel
    {
        private ProjectInfo _selectedProject = new();
        private ObservableCollection<ProjectInfo> _allProjects = new();

        private Stand _selectedStand;

        private readonly IProjectInfoRepository _projectRepository;

        public AllStandsViewModel(IProjectInfoRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public ObservableCollection<ProjectInfo> AllProjects
        {
            get => _allProjects;
            set => Set(ref _allProjects, value);
        }

        public ProjectInfo SelectedProject
        {
            get => _selectedProject;
            set => Set(ref _selectedProject, value);
        }

        public Stand SelectedStand
        {
            get => _selectedStand;
            set => Set(ref _selectedStand, value);
        }

        public Stand SelectedResult { get; set; }

        public void ConfirmSelection()
        {
            SelectedResult = SelectedStand;
        }

        public async Task GetAllProjectsAsync()
        {
            var projects = await _projectRepository.GetAllWithSandsAsync();

            AllProjects = projects.ToObservable();

            if (AllProjects.Any())
            {
                SelectedProject = AllProjects.First();
            }
        }
    }
}
