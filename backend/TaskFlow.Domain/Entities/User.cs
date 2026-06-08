using TaskFlow.Domain.Exceptions;

namespace TaskFlow.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;

    public ICollection<Project> Projects { get; private set; } = new List<Project>();

    // Construtor protegido para EF
    protected User() { }

    // Factory method — criação centralizada
    public static User Create(string name, string email, string passwordHash)
    {
        var normalizedName = (name ?? string.Empty).Trim();
        var normalizedEmail = (email ?? string.Empty).Trim().ToLowerInvariant();

        if (string.IsNullOrWhiteSpace(normalizedName))
            throw new ValidationException(nameof(Name), "Name is required.");

        if (string.IsNullOrWhiteSpace(normalizedEmail))
            throw new ValidationException(nameof(Email), "Email is required.");

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ValidationException(nameof(PasswordHash), "Password hash is required.");

        return new User
        {
            Id = Guid.NewGuid(),
            Name = normalizedName,
            Email = normalizedEmail,
            PasswordHash = passwordHash
        };
    }

    public void UpdateName(string name)
    {
        var normalized = (name ?? string.Empty).Trim();

        if (string.IsNullOrWhiteSpace(normalized))
            throw new ValidationException(nameof(Name), "Name is required.");

        Name = normalized;
    }
}
