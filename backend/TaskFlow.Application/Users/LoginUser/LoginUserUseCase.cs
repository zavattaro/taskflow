using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskFlow.Domain.Entities;
using TaskFlow.Infrastructure.Persistence;

namespace TaskFlow.Application.Users.LoginUser;

public sealed class LoginUserUseCase
{
    private readonly AppDbContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IConfiguration _configuration;

    public LoginUserUseCase(AppDbContext context, IPasswordHasher<User> passwordHasher, IConfiguration configuration)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _configuration = configuration;
    }

    public async Task<LoginUserResult?> ExecuteAsync(LoginUserCommand command)
    {
        var email = command.Email.Trim().ToLowerInvariant();
        var password = command.Password.Trim();

        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Email == email);

        if (user is null)
            return null;

        var passwordVerificationResult =
            _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

        if (passwordVerificationResult == PasswordVerificationResult.Failed)
            return null;

        var jwtKey = _configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("JWT key not configured.");

        var jwtIssuer = _configuration["Jwt:Issuer"]
            ?? throw new InvalidOperationException("JWT issuer not configured.");

        var jwtAudience = _configuration["Jwt:Audience"]
            ?? throw new InvalidOperationException("JWT audience not configured.");

        var expirationInHours = int.TryParse(
            _configuration["Jwt:ExpirationInHours"],
            out var hours
        )
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

        return new LoginUserResult(token, expiresAt);
    }
}
