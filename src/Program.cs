using api.Models;
using api.Services;

using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add In-Memory Database
builder.Services.AddDbContext<MinimalDbContext>(options =>
    options.UseInMemoryDatabase("minimalapisample"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

// Seeding In-Memory Database
using (var serviceScope = app.Services.CreateScope())
{
    var services = serviceScope.ServiceProvider;
    var context = services.GetRequiredService<MinimalDbContext>();
    context.Database.EnsureCreated();
}

app.MapControllers();

// Minimal Apis
app.MapGet("/", () => "This is a GET");
app.MapPost("/", () => "This is a POST");
app.MapPut("/", () => "This is a PUT");
app.MapDelete("/", () => "This is a DELETE");

var users = app.MapGroup("/users");

//users.MapGet("/", async (MinimalDbContext dbContext) =>
//    await dbContext.Users.ToListAsync());
users.MapGet("/", async (IUserService userService) =>
    await userService.ListUsersAsync());

//users.MapGet("/{id:int}", async (int id, MinimalDbContext dbContext, ILogger<Program> logger) =>
//{
//    logger.LogInformation("on '/users/{id}'", id);

//    var user = await dbContext.FindAsync<User>(id);
//    return user is null
//        ? Results.NotFound(new { Error = "This ID is notfound." })
//        : Results.Ok(user);
//});
users.MapGet("/{id:int}", async (int id, IUserService userService, ILogger<Program> logger) =>
{
    logger.LogInformation("on '/users/{id}'", id);

    var user = await userService.GetByIdAsync(id);
    return user is null
        ? Results.NotFound(new { Error = "This ID is notfound." })
        : Results.Ok(user);
});

users.MapPost("/", async (User? user, MinimalDbContext dbContext, ILogger<Program> logger) =>
{
    if (user is null)
    {
        return Results.BadRequest("Who are you?");
    }

    logger.LogInformation(user.ToString());

    var addedEntity = dbContext.Users.Add(user);
    dbContext.SaveChanges();

    return addedEntity is null
        ? Results.BadRequest("Failed to add.")
        : Results.Created($"/users/{addedEntity.Entity.Id}", addedEntity.Entity);
});

app.Run();
