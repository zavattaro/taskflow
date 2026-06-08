using TaskFlow.Domain.Enums;
using TaskFlow.Domain.Exceptions;

namespace TaskFlow.Domain.Entities;

public class TaskItem
{
    public Guid Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public TaskItemStatus Status { get; private set; } = TaskItemStatus.Todo;

    public Guid ProjectId { get; private set; }
    public Project Project { get; private set; } = null!;

    // Construtor protegido para EF
    protected TaskItem() { }

    // Factory method — criação centralizada
    public static TaskItem Create(Guid projectId, string title, string? description)
    {
        if (projectId == Guid.Empty)
            throw new ValidationException(nameof(ProjectId), "Project ID is required.");

        var normalizedTitle = (title ?? string.Empty).Trim();
        var normalizedDescription = string.IsNullOrWhiteSpace(description)
            ? null
            : description.Trim();

        if (string.IsNullOrWhiteSpace(normalizedTitle))
            throw new ValidationException(nameof(Title), "Title is required.");

        return new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = normalizedTitle,
            Description = normalizedDescription,
            ProjectId = projectId,
            Status = TaskItemStatus.Todo
        };
    }

    // Comportamento explícito de domínio
    public void UpdateStatus(TaskItemStatus newStatus)
    {
        Status = newStatus;
    }

    public void Update(string title, string? description)
    {
        var normalizedTitle = (title ?? string.Empty).Trim();

        if (string.IsNullOrWhiteSpace(normalizedTitle))
            throw new ValidationException(nameof(Title), "Title is required.");

        Title = normalizedTitle;
        Description = string.IsNullOrWhiteSpace(description)
            ? null
            : description.Trim();
    }
}
