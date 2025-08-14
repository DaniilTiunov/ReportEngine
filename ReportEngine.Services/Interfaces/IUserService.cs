using ReportEngine.Domain.Entities;

namespace ReportEngine.Services.Interfaces
{
    public interface IUserService<TUser>
        where TUser : User
    {
        Task<IEnumerable<TUser>> GetAllUsersAsync();
        Task<TUser> GetUserByIdAsync(int id);
        Task<TUser> CreateUserAsync(TUser entity);
        Task<TUser> UpdateUserAsync(TUser entity);
        Task DeleteUserAsync(User entity);
    }
}
