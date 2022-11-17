using api.Models;

namespace api.Services;

public interface IUserService
{
    Task<User?> GetByIdAsync(int id);
    Task<List<User>> ListUsersAsync();
}