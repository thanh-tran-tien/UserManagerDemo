namespace UserManagerDemo.Application.Users.Dtos;

public class UserDto
{
    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string? PhoneNumber { get; set; }
    public string? ZipCode { get; set; }
}