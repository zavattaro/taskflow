using TaskFlow.Domain.Enums;

namespace TaskFlow.Domain.Entities;

public class TaskItem
{
    public Guid Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public TaskItemsStatus Status { get; private set; } = TaskItemsStatus.Todo;

    public Guid ProjectId { get; private set; }
    public Project Project { get; private set; } = null!;

    // Construtor protegido para EF
    protected TaskItem() { }

    // Factory method — criação centralizada
    public static TaskItem Create(
        Guid projectId,
        string title,
        string? description
    )
    {
        var normalizedTitle = title.Trim();
        var normalizedDescription = string.IsNullOrWhiteSpace(description)
            ? null
            : description.Trim();

        return new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = normalizedTitle,
            Description = normalizedDescription,
            ProjectId = projectId,
            Status = TaskItemsStatus.Todo
        };
    }

    // Comportamento explícito de domínio
    public void UpdateStatus(TaskItemsStatus newStatus)
    {
        Status = newStatus;
    }
}
