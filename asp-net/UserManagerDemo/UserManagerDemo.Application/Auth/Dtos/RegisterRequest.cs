using UserManagerDemo.Application.Users.Dtos;

namespace UserManagerDemo.Application.Auth.Dtos;

public class RegisterRequest
{
    public UserDto User { get; set; }
    public string Password { get; set; } = default!;
}