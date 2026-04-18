namespace TaskFlow.Application.Tasks.CreateTaskItem;

public sealed record CreateTaskItemResult(
    Guid TaskItemId,
    string Title,
    string? Description,
    string Status,
    Guid ProjectId
);