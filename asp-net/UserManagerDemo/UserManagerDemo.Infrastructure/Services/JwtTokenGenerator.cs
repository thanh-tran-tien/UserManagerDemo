using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserManagerDemo.Application.Interface.Auth;
using UserManagerDemo.Domain.Entity;
using UserManagerDemo.Infrastructure.Persistence;

namespace UserManagerDemo.Infrastructure.Services;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly IConfiguration _configuration;
    private readonly AppDbContext _dbContext;

    public JwtTokenGenerator(IConfiguration configuration, AppDbContext dbContext)
    {
        _configuration = configuration;
        _dbContext = dbContext;
    }

    public Task<string> GenerateAccessTokenAsync(ApplicationUser user, bool rememberMe)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? "")
        };

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var minutes = rememberMe
            ? Convert.ToDouble(jwtSettings["RememberMeDurationInMinutes"])
            : Convert.ToDouble(jwtSettings["DurationInMinutes"]);

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"],
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(minutes),
            signingCredentials: creds
        );

        return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
    }

    public async Task<RefreshToken> GenerateRefreshTokenAsync(ApplicationUser user, bool rememberMe)
    {
        var refreshToken = new RefreshToken
        {
            Token = Guid.NewGuid().ToString("N"),
            Expires = rememberMe ? DateTime.UtcNow.AddDays(30) : DateTime.UtcNow.AddDays(7),
            UserId = user.Id
        };

        _dbContext.RefreshTokens.Add(refreshToken);
        await _dbContext.SaveChangesAsync();

        return refreshToken;
    }
}