namespace ReportEngine.Domain.Repositories.Interfaces
{
    internal interface IBaseRepository<TEntity> where TEntity : class
    {
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity> GetByIdAsync(int id);
        Task AddAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        Task<int> DeleteByIdAsync(int id);
    }
}
