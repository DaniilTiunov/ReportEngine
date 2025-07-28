using ReportEngine.Domain.Entities.BaseEntities;

namespace ReportEngine.Domain.Repositories.Interfaces
{
    public interface IGenericBaseRepository<T> where T : BaseEquip
    {
        Task AddAsync(T entity);
        Task DeleteAsync(T entity);
        Task<IEnumerable<T>> GetAllAsync();
    }
}
