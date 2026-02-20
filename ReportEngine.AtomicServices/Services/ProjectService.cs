using Mapster;
using ReportEngine.AtomicDomain.Repositories;
using ReportEngine.AtomicServices.Models;

namespace ReportEngine.AtomicServices.Services
{
    public class ProjectService
    {
        private readonly ProjectRepository _projectRepository;

        public ProjectService(ProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task<ProjectModel> GetProjectByIdAsync(int projectId)
        {
            var projectEntity = await _projectRepository.GetProjectAsync(pr => pr.Id == projectId);

            return projectEntity.Adapt<ProjectModel>();

        }
    }    
}
