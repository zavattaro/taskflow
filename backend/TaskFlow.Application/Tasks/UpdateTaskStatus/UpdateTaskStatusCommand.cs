namespace TaskFlow.Application.Tasks.UpdateTaskStatus;

public sealed record UpdateTaskStatusCommand(
    Guid ProjectId,
    Guid TaskItemId,
    string Status
);

