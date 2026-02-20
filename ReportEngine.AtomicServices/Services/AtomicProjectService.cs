using Mapster;
using ReportEngine.AtomicDomain.Entities;
using ReportEngine.AtomicDomain.Repositories;
using ReportEngine.AtomicServices.Models;

namespace ReportEngine.AtomicServices.Services
{
    public class AtomicProjectService
    {
        private readonly AtomicProjectRepository _projectRepository;

        public AtomicProjectService(AtomicProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public async Task AddNewProjectAsync(AtomicProjectModel projectModel)
        {
            var projectEntity = projectModel.Adapt<Project>();
            await _projectRepository.AddNewProject(projectEntity);
        }

        public async Task<AtomicProjectModel> GetProjectByIdAsync(int projectId)
        {
            var projectEntity = await _projectRepository.GetProjectAsync(pr => pr.Id == projectId);

            return projectEntity.Adapt<AtomicProjectModel>();

        }
        public async Task<IEnumerable<AtomicProjectModel>> GetAllProjectsAsync()
        {
            var projectEntities = await _projectRepository.GetAllProjectsAsync();
            return projectEntities.Adapt<IEnumerable<AtomicProjectModel>>();
        }
    }    
}
