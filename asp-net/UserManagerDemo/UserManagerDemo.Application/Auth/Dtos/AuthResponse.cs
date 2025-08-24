namespace UserManagerDemo.Application.Auth.Dtos;

public class AuthResponse
{
    public string Token { get; set; } = default!;
    public DateTime Expiration { get; set; }
    public string RefreshToken { get; set; } = default!;
    public DateTime RefreshTokenExpiration { get; set; }
}
