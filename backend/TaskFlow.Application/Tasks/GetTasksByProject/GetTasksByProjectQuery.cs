namespace TaskFlow.Application.Tasks.GetTasksByProject;

public sealed record GetTasksByProjectQuery(
    Guid ProjectId
);