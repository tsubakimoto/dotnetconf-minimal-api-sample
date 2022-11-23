using api.Models;

using Microsoft.EntityFrameworkCore;

namespace api.Services;

public class UserService : IUserService
{
    private readonly MinimalDbContext _dbContext;
    private readonly ILogger<UserService> _logger;

    public UserService(MinimalDbContext dbContext, ILogger<UserService> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<List<User>> ListUsersAsync() => await _dbContext.Users.ToListAsync();

    public async Task<User?> GetByIdAsync(int id) => await _dbContext.FindAsync<User>(id);

    public async Task<IResult> CreateAsync(User user)
    {
        if (user is null)
        {
            return Results.BadRequest("Who are you?");
        }
        if (string.IsNullOrWhiteSpace(user.Name))
        {
            return Results.BadRequest("Name is required.");
        }
        if (user.Name.Equals("admin", StringComparison.OrdinalIgnoreCase))
        {
            return Results.BadRequest("This name is reserved.");
        }

        if (await _dbContext.Users.AnyAsync(u => u.Name == user.Name))
        {
            return Results.BadRequest("This user is already exists.");
        }

        _logger.LogInformation(user.ToString());

        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        return Results.Created($"/users/{user.Id}", user);
    }
}
