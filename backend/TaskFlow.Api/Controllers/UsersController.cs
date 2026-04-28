using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskFlow.Api.Contracts.Users;
using TaskFlow.Api.Errors;
using TaskFlow.Domain.Entities;
using TaskFlow.Infrastructure.Persistence;

namespace TaskFlow.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IConfiguration _configuration;

    public UsersController(AppDbContext context, IPasswordHasher<User> passwordHasher, IConfiguration configuration)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginUserRequest request)
    {
        var email = request.Email?.Trim().ToLowerInvariant();
        var password = request.Password?.Trim();

        if (string.IsNullOrWhiteSpace(email))
            return BadRequest(new { message = ErrorMessages.EmailRequired });

        if (string.IsNullOrWhiteSpace(password))
            return BadRequest(new { message = ErrorMessages.PasswordRequired });

        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Email == email);

        if (user is null)
            return Unauthorized(new { message = ErrorMessages.InvalidCredentials });

        var passwordVerificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

        if (passwordVerificationResult == PasswordVerificationResult.Failed)
            return Unauthorized(new { message = ErrorMessages.InvalidCredentials });

        var jwtKey = _configuration["Jwt:Key"]
        ?? throw new InvalidOperationException(ErrorMessages.JwtKeyNotConfigured);

        var jwtIssuer = _configuration["Jwt:Issuer"]
            ?? throw new InvalidOperationException(ErrorMessages.JwtIssuerNotConfigured);

        var jwtAudience = _configuration["Jwt:Audience"]
            ?? throw new InvalidOperationException(ErrorMessages.JwtAudienceNotConfigured);

        var expirationInHours = int.TryParse(_configuration["Jwt:ExpirationInHours"], out var hours)
            ? hours
            : 2;

        var expiresAt = DateTime.UtcNow.AddHours(expirationInHours);

        var claims = new List<Claim>
    {
        new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
        new(JwtRegisteredClaimNames.Email, user.Email),
        new(JwtRegisteredClaimNames.UniqueName, user.Name),
        new(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new(ClaimTypes.Name, user.Name),
        new(ClaimTypes.Email, user.Email)
    };

        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: signingCredentials);

        var token = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

        var response = new LoginUserResponse
        {
            Token = token,
            ExpiresAt = expiresAt
        };

        return Ok(response);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterUserRequest request)
    {
        var name = request.Name?.Trim();
        var email = request.Email?.Trim().ToLowerInvariant();
        var password = request.Password?.Trim();

        if (string.IsNullOrWhiteSpace(name))
            return BadRequest(new { message = ErrorMessages.NameRequired });

        if (string.IsNullOrWhiteSpace(email))
            return BadRequest(new { message = ErrorMessages.EmailRequired });

        if (string.IsNullOrWhiteSpace(password))
            return BadRequest(new { message = ErrorMessages.PasswordRequired });

        var emailAlreadyExists = await _context.Users
            .AnyAsync(x => x.Email == email);

        if (emailAlreadyExists)
            return Conflict(new { message = ErrorMessages.EmailAlreadyRegistered });

        var user = new User
        {
            Id = Guid.NewGuid(),
            Name = name,
            Email = email
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, password);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        var response = new RegisterUserResponse
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email
        };

        return Created($"/api/users/{user.Id}", response);
    }
}