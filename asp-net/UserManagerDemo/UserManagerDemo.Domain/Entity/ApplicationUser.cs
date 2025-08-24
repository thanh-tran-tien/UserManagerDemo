using Microsoft.AspNetCore.Identity;

namespace UserManagerDemo.Domain.Entity;

public class ApplicationUser : IdentityUser<Guid>
{
    public ApplicationUserProfile? ApplicationUserProfile { get; set; }

    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}