using ReportEngine.Domain.Entities;
using ReportEngine.Domain.Repositories;
using ReportEngine.Domain.Repositories.Interfaces;
using ReportEngine.Services.Interfaces;

namespace ReportEngine.Services.Services;

public class UserService : IUserService<User>
{
    private readonly UserRepository _repository;

    public UserService(UserRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _repository.GetAllAsync();
    }

    public async Task<User> GetUserByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<User> CreateUserAsync(User entity)
    {
        await _repository.AddAsync(entity);
        return entity;
    }

    public async Task<User> UpdateUserAsync(User entity)
    {
        await _repository.UpdateAsync(entity);
        return entity;
    }

    public async Task DeleteUserAsync(User entity)
    {
        await _repository.DeleteAsync(entity);
    }
}