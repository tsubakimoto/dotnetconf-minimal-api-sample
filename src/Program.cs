using api;
using api.Models;
using api.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<SwaggerGeneratorOptions>(options =>
{
    options.InferSecuritySchemes = true;
});

// Add In-Memory Database
builder.Services.AddDbContext<MinimalDbContext>(options =>
    options.UseInMemoryDatabase("minimalapisample"));

// Add authentication and authorization
builder.Services.AddAuthentication().AddJwtBearer();
builder.Services.AddAuthorization(options =>
    options.AddPolicy("AdminsOnly", b => b.RequireClaim("admin", "true")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    // Seeding In-Memory Database
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<MinimalDbContext>();
    context.Database.EnsureCreated();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Minimal Apis
app.MapGet("/", () => "This is a GET"); // HTTP GET
app.MapPost("/", () => "This is a POST"); // HTTP POST
app.MapPut("/", () => "This is a PUT"); // HTTP PUT
app.MapDelete("/", () => "This is a DELETE"); // HTTP DELETE

app.MapGet("/admin", () => "You are Administrator")
    .RequireAuthorization("AdminsOnly")
    .EnableOpenApiWithAuthentication();

app.MapGet("/array", (string[] tags) =>
    $"count:{tags.Count()}, tag1:{tags[0]}, tag2:{tags[1]}");

// app.MapGet("/users",
//     async (MinimalDbContext dbContext) =>
//         await dbContext.Users.ToListAsync());

var users = app.MapGroup("/users");

// users.MapGet("/",
//     async (MinimalDbContext dbContext) =>
//         await dbContext.Users.ToListAsync());

users.MapGet("/", async (IUserService userService) =>
    await userService.ListUsersAsync());

// users.MapGet("/{id:int}",
//     async (int id, MinimalDbContext dbContext) =>
//         await dbContext.Users.FindAsync(id) is User user
//             ? Results.Ok(user)
//             : Results.NotFound()
//     );

//users.MapGet("/{id:int}", async (int id, MinimalDbContext dbContext, ILogger<Program> logger) =>
//{
//    logger.LogInformation("on '/users/{id}'", id);

//    var user = await dbContext.FindAsync<User>(id);
//    return user is null
//        ? Results.NotFound(new { Error = "This ID is notfound." })
//        : Results.Ok(user);
//});

users.MapGet("/{id:int}",
    async (int id, IUserService userService, ILogger<Program> logger) =>
    {
        logger.LogInformation("on '/users/{id}'", id);

        var user = await userService.GetByIdAsync(id);
        return user is null
            ? Results.NotFound(new { Error = "This ID is notfound." })
            : Results.Ok(user);
    })
    .WithOpenApi(generatedOperation =>
    {
        var parameter = generatedOperation.Parameters[0];
        parameter.Description = "This is user ID";
        return generatedOperation;
    });

// users.MapGet("/{id:int}",
//     async ([AsParameters] UserDetailsRequest request) =>
// {
//     request.Logger.LogInformation("on '/users/{id}'", request.Id);

//     var user = await request.UserService.GetByIdAsync(request.Id);
//     return user is null
//         ? Results.NotFound(new { Error = "This ID is notfound." })
//         : Results.Ok(user);
// });

// users.MapPost("/",
//     async ([FromBody] User user, [FromServices] MinimalDbContext dbContext) =>
//     {
//         dbContext.Users.Add(user);
//         await dbContext.SaveChangesAsync();

//         return Results.Created($"/users/{user.Id}", user);
//     });

// users.MapPost("/",
//     async ([FromBody] User user,
//             [FromServices] MinimalDbContext dbContext,
//             [FromServices] ILogger<Program> logger) =>
//     {
//         if (user is null)
//         {
//             return Results.BadRequest("Who are you?");
//         }
//         if (string.IsNullOrWhiteSpace(user.Name))
//         {
//             return Results.BadRequest("Name is required.");
//         }
//         if (user.Name.Equals("admin", StringComparison.OrdinalIgnoreCase))
//         {
//             return Results.BadRequest("This name is reserved.");
//         }

//         if (await dbContext.Users.AnyAsync(u => u.Name == user.Name))
//         {
//             return Results.BadRequest("This user is already exists.");
//         }

//         logger.LogInformation(user.ToString());

//         dbContext.Users.Add(user);
//         await dbContext.SaveChangesAsync();

//         return Results.Created($"/users/{user.Id}", user);
//     })
//     .RequireAuthorization("AdminsOnly")
//     .EnableOpenApiWithAuthentication();

users.MapPost("/",
    async ([FromBody] User user,
            [FromServices] IUserService userService) =>
        await userService.CreateAsync(user))
    .RequireAuthorization("AdminsOnly")
    .EnableOpenApiWithAuthentication();

var foods = app.MapGroup("/foods")
    .RequireAuthorization("AdminsOnly")
    .EnableOpenApiWithAuthentication();
foods.MapGet("/", () => "...");
foods.MapGet("/{id:int}", () => "...");
foods.MapPost("/", () => "...");
foods.MapPut("/", () => "...");
foods.MapDelete("/", () => "...");

app.Run();
