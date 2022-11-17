using api.Models;
using api.Services;

using Microsoft.EntityFrameworkCore;

namespace api.test.Services;

public class UserServiceTest
{
    [Fact]
    void ListUsersAsyncTest()
    {
        MinimalDbContext dbContext = GetDbContext();
        UserService service = new(dbContext);

        List<User> actual = service.ListUsersAsync().Result;
        Assert.Equal(5, actual.Count);
    }

    MinimalDbContext GetDbContext()
    {
        DbContextOptionsBuilder<MinimalDbContext> builder = new();
        builder.UseInMemoryDatabase("minimalapisample");

        MinimalDbContext dbContext = new(builder.Options);
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();

        return dbContext;
    }
}
