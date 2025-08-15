using ReportEngine.Domain.Entities;

namespace ReportEngine.Domain.Repositories.Interfaces;

public interface IFormedDrainagesRepository : IBaseRepository<FormedDrainage>
{
    Task<IEnumerable<FormedDrainage>> GetAllWithPurposesAsync();
    Task<FormedDrainage> GetByIdWithPurposesAsync(int id);
}