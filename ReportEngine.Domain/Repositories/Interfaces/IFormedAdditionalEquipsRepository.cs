using ReportEngine.Domain.Entities;

namespace ReportEngine.Domain.Repositories.Interfaces;

public interface IFormedAdditionalEquipsRepository
{
    Task<IEnumerable<FormedAdditionalEquip>> GetAllAsync();
    Task AddAsync(FormedAdditionalEquip entity);
    
    Task<IEnumerable<FormedAdditionalEquip>> GetAllWithPurposesAsync();
    Task<FormedAdditionalEquip> GetByIdWithPurposesAsync(int id);
}