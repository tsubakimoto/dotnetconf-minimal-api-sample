using api.Models;
using api.Services;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Moq;

namespace api.test.Services;

public class UserServiceTest
{
    [Fact]
    void ListUsersAsyncTest()
    {
        UserService service = GetService();

        List<User> actual = service.ListUsersAsync().Result;
        Assert.Equal(5, actual.Count);
    }

    [Fact]
    void GetByIdAsyncTest_Ok()
    {
        UserService service = GetService();

        User? actual = service.GetByIdAsync(1).Result;
        Assert.NotNull(actual);
        Assert.Equal(1, actual.Id);
        Assert.Equal("name 1", actual.Name);
    }

    [Fact]
    void GetByIdAsyncTest_NotFound()
    {
        UserService service = GetService();

        User? actual = service.GetByIdAsync(100).Result;
        Assert.Null(actual);
    }

    [Fact]
    void CreateAsync_NG1()
    {
        UserService service = GetService();

        var actual = service.CreateAsync(null).Result as BadRequest<string>;
        Assert.Equal(400, actual!.StatusCode);
        Assert.Equal("Who are you?", actual!.Value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    void CreateAsync_NG2(string? name)
    {
        UserService service = GetService();

        var actual = service.CreateAsync(new() { Name = name }).Result as BadRequest<string>;
        Assert.Equal(400, actual!.StatusCode);
        Assert.Equal("Name is required.", actual!.Value);
    }

    [Fact]
    void CreateAsync_NG3()
    {
        UserService service = GetService();

        var actual = service.CreateAsync(new() { Name = "admin" }).Result as BadRequest<string>;
        Assert.Equal(400, actual!.StatusCode);
        Assert.Equal("This name is reserved.", actual!.Value);
    }

    [Fact]
    void CreateAsync_NG4()
    {
        UserService service = GetService();

        var actual = service.CreateAsync(new() { Name = "name 1" }).Result as BadRequest<string>;
        Assert.Equal(400, actual!.StatusCode);
        Assert.Equal("This user is already exists.", actual!.Value);
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

    UserService GetService() => new(GetDbContext(), new Mock<ILogger<UserService>>().Object);
}
