using FluentResults;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using UserManagerDemo.Application.Auth.Dtos;
using UserManagerDemo.Application.Interface.Auth;
using UserManagerDemo.Domain.Entity;
using UserManagerDemo.Infrastructure.Persistence;

namespace UserManagerDemo.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IValidator<LoginRequest> _loginRequestValidator;
    private readonly IValidator<RegisterRequest> _registerRequestValidator;
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        IJwtTokenGenerator jwtTokenGenerator,
        IValidator<RegisterRequest> registerRequestValidator,
        IValidator<LoginRequest> loginRequestValidator,
        AppDbContext dbContext)
    {
        _userManager = userManager;
        _jwtTokenGenerator = jwtTokenGenerator;
        _registerRequestValidator = registerRequestValidator;
        _loginRequestValidator = loginRequestValidator;
        _dbContext = dbContext;
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var validationResult = await _loginRequestValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            return BadRequest(Result.Fail(errors));
        }

        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return Unauthorized(Result.Fail("Invalid email or password"));

        var checkPassword = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!checkPassword)
            return Unauthorized(Result.Fail("Invalid email or password"));

        var accessToken = await _jwtTokenGenerator.GenerateAccessTokenAsync(user, request.RememberMe);
        var refreshToken = await _jwtTokenGenerator.GenerateRefreshTokenAsync(user, request.RememberMe);

        Response.Cookies.Append("jwt", accessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTimeOffset.UtcNow.AddMinutes(request.RememberMe ? 60 : 30)
        });

        Response.Cookies.Append("refreshToken", refreshToken.Token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = refreshToken.Expires
        });

        return Ok(Result.Ok(new AuthResponse
        {
            Token = accessToken,
            Expiration = refreshToken.Expires,
            RefreshToken = refreshToken.Token,
            RefreshTokenExpiration = refreshToken.Expires
        }));
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return Unauthorized();

        var tokens = await _dbContext.RefreshTokens
            .Where(rt => rt.UserId == user.Id && !rt.IsRevoked && rt.Expires > DateTime.UtcNow)
            .ToListAsync();

        foreach (var t in tokens)
        {
            t.IsRevoked = true;
            t.RevokedAt = DateTime.UtcNow;
        }

        await _dbContext.SaveChangesAsync();

        Response.Cookies.Delete("jwt");
        Response.Cookies.Delete("refreshToken");

        return Ok(Result.Ok().WithSuccess("Logged out successfully"));
    }

    [HttpPost]
    public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
    {
        var refreshToken = await _userManager.Users
            .SelectMany(u => u.RefreshTokens)
            .Include(x => x.User)
            .FirstOrDefaultAsync(rt => rt.Token == request.RefreshToken);

        if (refreshToken == null || refreshToken.IsUsed || refreshToken.IsRevoked || refreshToken.Expires < DateTime.UtcNow)
            return Unauthorized(Result.Fail("Invalid refresh token"));

        refreshToken.IsUsed = true;
        await _userManager.UpdateAsync(refreshToken.User);

        var newAccessToken = await _jwtTokenGenerator.GenerateAccessTokenAsync(refreshToken.User, rememberMe: true);
        var newRefreshToken = await _jwtTokenGenerator.GenerateRefreshTokenAsync(refreshToken.User, rememberMe: true);

        Response.Cookies.Delete("jwt");
        Response.Cookies.Delete("refreshToken");

        Response.Cookies.Append("jwt", newAccessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = DateTimeOffset.UtcNow.AddMinutes(60)
        });

        Response.Cookies.Append("refreshToken", newRefreshToken.Token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Expires = newRefreshToken.Expires
        });

        return Ok(Result.Ok(new AuthResponse
        {
            Token = newAccessToken,
            Expiration = newRefreshToken.Expires,
            RefreshToken = newRefreshToken.Token,
            RefreshTokenExpiration = newRefreshToken.Expires
        }));
    }

    [HttpPost]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var validationResult = await _registerRequestValidator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            return BadRequest(Result.Fail(errors));
        }

        var user = new ApplicationUser
        {
            UserName = request.User.Email,
            Email = request.User.Email,
            PhoneNumber = request.User.PhoneNumber,
            ApplicationUserProfile = new ApplicationUserProfile
            {
                FirstName = request.User.FirstName,
                LastName = request.User.LastName,
                Email = request.User.Email,
                PhoneNumber = request.User.PhoneNumber,
                ZipCode = request.User.ZipCode
            }
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
            return BadRequest(Result.Fail(string.Join("; ", result.Errors.Select(e => e.Description))));

        return Ok(Result.Ok().WithSuccess("User registered successfully"));
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> CurrentUser()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        var user = _dbContext.ApplicationUserProfiles.Where(x => x.UserId.ToString() == userId)
            .Include(x => x.ApplicationUser)
            .FirstOrDefault();

        return Ok(Result.Ok(new
        {
            id = user.ApplicationUser.Id,
            userProfileId= user.Id,
            email = user.Email,
            firstName = user.FirstName,
            lastName = user.LastName
        }));
    }
}