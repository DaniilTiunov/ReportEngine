using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportEngine.App.ViewModels
{
    public class ProjectViewModel : BaseViewModel
    {
        private readonly IBaseRepository<ProjectInfo> _projectRepository;

        private ObservableCollection<ProjectInfo> _allProjects;

        public ObservableCollection<ProjectInfo> AllProjects 
        { 
            get => _allProjects; 
            set => Set(ref _allProjects, value); 
        }

        public ProjectViewModel(IBaseRepository<ProjectInfo> projectRepository)
        {
            _projectRepository = projectRepository;
        }
    }
}
