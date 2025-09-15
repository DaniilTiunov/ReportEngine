using ReportEngine.Domain.Entities;

namespace ReportEngine.Domain.Repositories.Interfaces;

public interface IFormedElectricalRepository
{
    Task<IEnumerable<FormedElectricalComponent>> GetAllAsync();
    Task AddAsync(FormedElectricalComponent entity);
    Task<IEnumerable<FormedElectricalComponent>> GetAllWithPurposesAsync();
    Task<FormedElectricalComponent> GetByIdWithPurposesAsync(int id);
    Task UpdateAsync(ElectricalPurpose purpose);
    Task DeletePurposeAsync(int purposeId);
}