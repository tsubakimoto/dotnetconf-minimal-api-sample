using api.Models;

namespace api.Services;

public interface IUserService
{
    Task<List<User>> ListUsersAsync();
}