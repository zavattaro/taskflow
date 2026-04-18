namespace TaskFlow.Application.Tasks.UpdateTaskStatus;

public sealed record UpdateTaskStatusResult(
    Guid TaskItemId,
    string Title,
    string? Description,
    string Status,
    Guid ProjectId
);