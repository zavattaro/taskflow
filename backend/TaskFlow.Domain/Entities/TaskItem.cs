using TaskFlow.Domain.Enums;

namespace TaskFlow.Domain.Entities;

public class TaskItem
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskItemsStatus Status { get; set; } = TaskItemsStatus.Todo;

    public Guid ProjectId { get; set; }
    public Project Project { get; set; } = null!;
}
