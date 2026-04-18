namespace TaskFlow.Application.Projects.CreateProject;

public sealed record CreateProjectResult(
    Guid ProjectId,
    string Name,
    string? Description
    );
