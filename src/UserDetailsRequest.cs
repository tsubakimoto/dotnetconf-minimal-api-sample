using api.Services;

namespace api;

class UserDetailsRequest
{
    public int Id { get; set; }
    public IUserService UserService { get; set; } = null!;
    public ILogger<Program> Logger { get; set; } = null!;
}