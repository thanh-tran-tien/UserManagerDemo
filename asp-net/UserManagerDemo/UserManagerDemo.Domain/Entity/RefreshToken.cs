using UserManagerDemo.Domain.Common;

namespace UserManagerDemo.Domain.Entity;

public class RefreshToken : IEntity<Guid>
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Token { get; set; } = default!;
    public DateTime Expires { get; set; }
    public bool IsRevoked { get; set; } = false;
    public DateTime? RevokedAt { get; set; }
    public bool IsUsed { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = default!;
}