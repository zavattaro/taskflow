using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Infrastructure.Persistence;

namespace TaskFlow.Application.Users.RegisterUser;

public sealed class RegisterUserUseCase
{
    private readonly AppDbContext _context;
    private readonly IPasswordHasher<User> _passwordHasher;

    public RegisterUserUseCase(AppDbContext context, IPasswordHasher<User> passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task<RegisterUserResult> ExecuteAsync(RegisterUserCommand command)
    {
        var name = command.Name.Trim();
        var email = command.Email.Trim().ToLowerInvariant();

        var emailAlreadyExists = await _context.Users
            .AnyAsync(x => x.Email == email);

        if (emailAlreadyExists)
            throw new DomainException("Email is already registered.");

        var passwordHash = _passwordHasher.HashPassword(null!, command.Password);
        var user = User.Create(name, email, passwordHash);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return new RegisterUserResult(user.Id, user.Name, user.Email);
    }
}
