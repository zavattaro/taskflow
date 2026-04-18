namespace TaskFlow.Application.Projects.CreateProject;

public sealed record CreateProjectCommand(
    Guid UserId,
    string Name,
    string? Description
);


