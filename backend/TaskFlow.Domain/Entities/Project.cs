using TaskFlow.Domain.Exceptions;

namespace TaskFlow.Domain.Entities;

public class Project
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public Guid UserId { get; private set; }

    public User User { get; private set; } = null!;
    public ICollection<TaskItem> Tasks { get; private set; } = new List<TaskItem>();

    // Construtor protegido para EF
    protected Project() { }

    // Factory method — criação centralizada
    public static Project Create(Guid userId, string name, string? description)
    {
        if (userId == Guid.Empty)
            throw new ValidationException(nameof(UserId), "User ID is required.");

        var normalizedName = (name ?? string.Empty).Trim();
        var normalizedDescription = string.IsNullOrWhiteSpace(description)
            ? null
            : description.Trim();

        if (string.IsNullOrWhiteSpace(normalizedName))
            throw new ValidationException(nameof(Name), "Name is required.");

        return new Project
        {
            Id = Guid.NewGuid(),
            Name = normalizedName,
            Description = normalizedDescription,
            UserId = userId
        };
    }

    public void Update(string name, string? description)
    {
        var normalizedName = (name ?? string.Empty).Trim();

        if (string.IsNullOrWhiteSpace(normalizedName))
            throw new ValidationException(nameof(Name), "Name is required.");

        Name = normalizedName;
        Description = string.IsNullOrWhiteSpace(description)
            ? null
            : description.Trim();
    }
}
