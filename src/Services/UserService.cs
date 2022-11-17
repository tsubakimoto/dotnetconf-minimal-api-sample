using api.Models;

using Microsoft.EntityFrameworkCore;

namespace api.Services;

public class UserService : IUserService
{
    private readonly MinimalDbContext _dbContext;

    public UserService(MinimalDbContext dbContext) => _dbContext = dbContext;

    public async Task<List<User>> ListUsersAsync() => await _dbContext.Users.ToListAsync();

    public async Task<User?> GetByIdAsync(int id) => await _dbContext.FindAsync<User>(id);
}
