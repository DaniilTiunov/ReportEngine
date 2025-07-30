using ReportEngine.Domain.Entities.BaseEntities.Interface;

namespace ReportEngine.Domain.Repositories.Interfaces
{
    public interface IGenericBaseRepository<T, TEntity> 
        where T : IBaseEquip
        where TEntity : class
    {
        Task AddAsync(TEntity entity);
        Task DeleteAsync(TEntity entity);
        Task<IEnumerable<TEntity>> GetAllAsync();
    }
}
