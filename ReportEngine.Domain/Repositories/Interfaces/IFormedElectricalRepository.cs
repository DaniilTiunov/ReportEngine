using ReportEngine.Domain.Entities;

namespace ReportEngine.Domain.Repositories.Interfaces;

public interface IFormedElectricalRepository
{
    Task<IEnumerable<FormedElectricalComponent>> GetAllAsync();
    
    Task AddAsync(FormedElectricalComponent entity);
}