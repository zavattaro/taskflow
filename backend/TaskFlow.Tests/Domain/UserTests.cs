using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Exceptions;

namespace TaskFlow.Tests.Domain;

public class UserTests
{
    [Fact]
    public void Create_WithValidData_ShouldReturnUser()
    {
        var user = User.Create("Enio Zavattaro", "enio@test.com", "hashed123");

        Assert.NotEqual(Guid.Empty, user.Id);
        Assert.Equal("Enio Zavattaro", user.Name);
        Assert.Equal("enio@test.com", user.Email);
        Assert.Equal("hashed123", user.PasswordHash);
    }

    [Fact]
    public void Create_ShouldNormalizeName()
    {
        var user = User.Create("  Enio  ", "enio@test.com", "hash");

        Assert.Equal("Enio", user.Name);
    }

    [Fact]
    public void Create_ShouldLowercaseEmail()
    {
        var user = User.Create("Enio", "ENIO@TEST.COM", "hash");

        Assert.Equal("enio@test.com", user.Email);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithInvalidName_ShouldThrowValidationException(string? name)
    {
        var ex = Assert.Throws<ValidationException>(() =>
            User.Create(name!, "enio@test.com", "hash"));

        Assert.Equal("Name", ex.Field);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Create_WithInvalidEmail_ShouldThrowValidationException(string? email)
    {
        var ex = Assert.Throws<ValidationException>(() =>
            User.Create("Enio", email!, "hash"));

        Assert.Equal("Email", ex.Field);
    }

    [Fact]
    public void UpdateName_WithValidName_ShouldUpdate()
    {
        var user = User.Create("Enio", "enio@test.com", "hash");

        user.UpdateName("Zavattaro");

        Assert.Equal("Zavattaro", user.Name);
    }

    [Fact]
    public void UpdateName_WithEmptyName_ShouldThrow()
    {
        var user = User.Create("Enio", "enio@test.com", "hash");

        Assert.Throws<ValidationException>(() => user.UpdateName(""));
    }
}
