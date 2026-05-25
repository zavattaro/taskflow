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
        var normalizedName = name.Trim();
        var normalizedDescription = string.IsNullOrWhiteSpace(description)
            ? null
            : description.Trim();

        return new Project
        {
            Id = Guid.NewGuid(),
            Name = normalizedName,
            Description = normalizedDescription,
            UserId = userId
        };
    }
}