using UserManagerDemo.Domain.Entity;

namespace UserManagerDemo.Application.Interface.Auth;

public interface IJwtTokenGenerator
{
    Task<string> GenerateAccessTokenAsync(ApplicationUser user, bool rememberMe);

    Task<RefreshToken> GenerateRefreshTokenAsync(ApplicationUser user, bool rememberMe);
}