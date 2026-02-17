using ReportEngine.Domain.Entities.BaseEntities.Interface;

namespace ReportEngine.Domain.Repositories.Interfaces
{
    public interface IPurposesRepository<T>
        where T : IPurposeEntity
    {
        Task UpdateAsync(T entity);
    }
}
