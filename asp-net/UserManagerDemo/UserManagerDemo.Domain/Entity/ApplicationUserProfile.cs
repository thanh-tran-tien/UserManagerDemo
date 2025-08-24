using UserManagerDemo.Domain.Common;

namespace UserManagerDemo.Domain.Entity;

public class ApplicationUserProfile : IEntity<Guid>
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }

    public string FirstName { get; set; } = default!;
    public string LastName { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string? PhoneNumber { get; set; }
    public string? ZipCode { get; set; }

    // convenience property (not mapped if EF)
    public string FullName => $"{FirstName} {LastName}";

    public ApplicationUser ApplicationUser { get; set; }
}