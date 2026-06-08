using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
    private readonly LoginUserUseCase _loginUserUseCase;

    public UsersController(
        AppDbContext context,
        IPasswordHasher<User> passwordHasher,
        LoginUserUseCase loginUserUseCase)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _loginUserUseCase = loginUserUseCase;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginUserRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
            return BadRequest(new { message = ErrorMessages.EmailRequired });

        if (string.IsNullOrWhiteSpace(request.Password))
            return BadRequest(new { message = ErrorMessages.PasswordRequired });

        var command = new LoginUserCommand(
            request.Email,
            request.Password
        );

        var result = await _loginUserUseCase.ExecuteAsync(command);

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

        // Hash antes de criar — PasswordHasher<User> não usa a instância internamente
        var passwordHash = _passwordHasher.HashPassword(null!, password);

        var user = Domain.Entities.User.Create(name, email, passwordHash);

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
