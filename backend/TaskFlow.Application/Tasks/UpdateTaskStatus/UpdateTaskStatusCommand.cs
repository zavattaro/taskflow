namespace TaskFlow.Application.Tasks.UpdateTaskStatus;

public sealed record UpdateTaskStatusCommand(
    Guid ProjectId,
    Guid UserId,
    Guid TaskItemId,
    string Status
);
