using FluentResults;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManagerDemo.Application.Auth.Dtos;
using UserManagerDemo.Application.Interface.Auth;
using UserManagerDemo.Domain.Entity;

namespace UserManagerDemo.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IValidator<RegisterRequest> _registerRequestValidator;
    private readonly IValidator<LoginRequest> _loginRequestValidator;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        IJwtTokenGenerator jwtTokenGenerator,
        IValidator<RegisterRequest> registerRequestValidator,
        IValidator<LoginRequest> loginRequestValidator)
    {
        _userManager = userManager;
        _jwtTokenGenerator = jwtTokenGenerator;
        _registerRequestValidator = registerRequestValidator;
        _loginRequestValidator = loginRequestValidator;
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

        return Ok(Result.Ok(new AuthResponse
        {
            Token = accessToken,
            Expiration = refreshToken.Expires, // access token expire khác, có thể set riêng
            RefreshToken = refreshToken.Token,
            RefreshTokenExpiration = refreshToken.Expires
        }));
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

        return Ok(Result.Ok(new AuthResponse
        {
            Token = newAccessToken,
            Expiration = newRefreshToken.Expires,
            RefreshToken = newRefreshToken.Token,
            RefreshTokenExpiration = newRefreshToken.Expires
        }));
    }
}