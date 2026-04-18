namespace TaskFlow.Application.Tasks.CreateTaskItem;

public sealed record CreateTaskItemCommand(
    Guid ProjectId,
    Guid UserId,
    string Title,
    string? Description
);