using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using TaskFlow.Application.Users.LoginUser;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Tests.Helpers;
using AuthException = TaskFlow.Domain.Exceptions.AuthenticationException;

namespace TaskFlow.Tests.Application;

public class LoginUserUseCaseTests
{
    private readonly IPasswordHasher<User> _hasher = new PasswordHasher<User>();

    private IConfiguration BuildConfig() => new ConfigurationBuilder()
        .AddInMemoryCollection(new Dictionary<string, string?>
        {
            ["Jwt:Key"] = "SuperSecretKeyForTestingPurposes12345!",
            ["Jwt:Issuer"] = "TaskFlow.Tests",
            ["Jwt:Audience"] = "TaskFlow.Tests",
            ["Jwt:ExpirationInHours"] = "1"
        })
        .Build();

    private async Task<User> SeedUserAsync(Infrastructure.Persistence.AppDbContext context, string email, string password)
    {
        var hash = _hasher.HashPassword(null!, password);
        var user = User.Create("Test User", email, hash);
        context.Users.Add(user);
        await context.SaveChangesAsync();
        return user;
    }

    [Fact]
    public async Task Execute_WithValidCredentials_ShouldReturnToken()
    {
        using var context = TestDbContextFactory.Create();
        await SeedUserAsync(context, "enio@test.com", "Senha123!");

        var useCase = new LoginUserUseCase(context, _hasher, BuildConfig());

        var result = await useCase.ExecuteAsync(
            new LoginUserCommand("enio@test.com", "Senha123!"));

        Assert.NotEmpty(result.Token);
        Assert.True(result.ExpiresAt > DateTime.UtcNow);
    }

    [Fact]
    public async Task Execute_WithWrongPassword_ShouldThrow()
    {
        using var context = TestDbContextFactory.Create();
        await SeedUserAsync(context, "enio@test.com", "Senha123!");

        var useCase = new LoginUserUseCase(context, _hasher, BuildConfig());

        await Assert.ThrowsAsync<AuthException>(() =>
            useCase.ExecuteAsync(
                new LoginUserCommand("enio@test.com", "WrongPassword")));
    }

    [Fact]
    public async Task Execute_WithNonExistentEmail_ShouldThrow()
    {
        using var context = TestDbContextFactory.Create();
        var useCase = new LoginUserUseCase(context, _hasher, BuildConfig());

        await Assert.ThrowsAsync<AuthException>(() =>
            useCase.ExecuteAsync(
                new LoginUserCommand("naoexiste@test.com", "Senha123!")));
    }
}
