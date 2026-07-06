using ReportEngine.Domain.Entities;

namespace ReportEngine.Domain.Repositories.Interfaces;

public interface IUserRepository : IBaseRepository<User>
{
    Task<User?> GetByUserLoginAsync(string login);
}
