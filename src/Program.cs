using api;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Minimal Apis
app.MapGet("/", () => "This is a GET");
app.MapPost("/", () => "This is a POST");
app.MapPut("/", () => "This is a PUT");
app.MapDelete("/", () => "This is a DELETE");

var users = app.MapGroup("/users");
users.MapGet("/{id:int}", (int id, ILogger<Program> logger) =>
{
    logger.LogInformation("on '/users/{id}'", id);

    if (0 < id)
    {
        return Results.Ok(new User { Id = id, Name = $"name {id}" });
    }
    else
    {
        return Results.BadRequest(new { Error = "This ID is invalid." });
    }
});

app.Run();
