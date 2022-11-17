using api.Models;
using api.Services;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
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

    [Fact]
    void GetByIdAsyncTest_Ok()
    {
        MinimalDbContext dbContext = GetDbContext();
        UserService service = new(dbContext);

        User? actual = service.GetByIdAsync(1).Result;
        Assert.NotNull(actual);
        Assert.Equal(1, actual.Id);
        Assert.Equal("name 1", actual.Name);
    }

    [Fact]
    void GetByIdAsyncTest_NotFound()
    {
        MinimalDbContext dbContext = GetDbContext();
        UserService service = new(dbContext);

        User? actual = service.GetByIdAsync(100).Result;
        Assert.Null(actual);
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
