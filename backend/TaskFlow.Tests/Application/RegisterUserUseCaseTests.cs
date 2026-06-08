using Microsoft.AspNetCore.Identity;
using TaskFlow.Application.Users.RegisterUser;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Tests.Helpers;

namespace TaskFlow.Tests.Application;

public class RegisterUserUseCaseTests
{
    private readonly IPasswordHasher<User> _hasher = new PasswordHasher<User>();

    [Fact]
    public async Task Execute_WithValidData_ShouldCreateUser()
    {
        using var context = TestDbContextFactory.Create();
        var useCase = new RegisterUserUseCase(context, _hasher);

        var result = await useCase.ExecuteAsync(
            new RegisterUserCommand("Enio", "enio@test.com", "Senha123!"));

        Assert.NotEqual(Guid.Empty, result.UserId);
        Assert.Equal("Enio", result.Name);
        Assert.Equal("enio@test.com", result.Email);

        var saved = await context.Users.FindAsync(result.UserId);
        Assert.NotNull(saved);
        Assert.NotEmpty(saved.PasswordHash);
    }

    [Fact]
    public async Task Execute_WithDuplicateEmail_ShouldThrowDomainException()
    {
        using var context = TestDbContextFactory.Create();
        var useCase = new RegisterUserUseCase(context, _hasher);

        await useCase.ExecuteAsync(
            new RegisterUserCommand("Enio", "enio@test.com", "Senha123!"));

        await Assert.ThrowsAsync<DomainException>(() =>
            useCase.ExecuteAsync(
                new RegisterUserCommand("Outro", "enio@test.com", "Senha456!")));
    }
}
