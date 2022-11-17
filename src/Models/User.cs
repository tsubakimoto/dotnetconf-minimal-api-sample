namespace api.Models;

public class User
{
    public int Id { get; set; } = 0;
    public string Name { get; set; } = string.Empty;

    public override string ToString()
    {
        return $"My ID is {Id} and name is {Name}.";
    }
}
