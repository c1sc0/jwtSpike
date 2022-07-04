namespace jwtSpike.Models;

public class User
{
    public Guid Id { get; set; }
    public string? Name { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
    public string? Password { get; set; }
}