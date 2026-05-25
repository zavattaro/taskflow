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
using TaskFlow.Application.Users.LoginUser;
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
    public async Task<IActionResult> Login(
    LoginUserRequest request,
    [FromServices] LoginUserUseCase loginUserUseCase)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
            return BadRequest(new { message = ErrorMessages.EmailRequired });

        if (string.IsNullOrWhiteSpace(request.Password))
            return BadRequest(new { message = ErrorMessages.PasswordRequired });

        var command = new LoginUserCommand(
            request.Email,
            request.Password
        );

        var result = await loginUserUseCase.ExecuteAsync(command);

        if (result is null)
            return Unauthorized(new { message = ErrorMessages.InvalidCredentials });

        return Ok(new LoginUserResponse
        {
            Token = result.Token,
            ExpiresAt = result.ExpiresAt
        });
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